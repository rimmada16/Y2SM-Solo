using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AStarPathfinding
{
    
public class Pathfinding : MonoBehaviour {

    public static event ExploderEvent Explode;
    public static event MeleeAttackEvent Melee;
    
    public static event RangedAttackEvent Ranged;
    
    
    // Add another enum for the enemy type
    // Depending on type logic for attack
    // Such as sword will run animation
    // Bow will instantiate arrow
    // Exploder will start explosion coroutine
    // Like a MC Creeper - Get slightly bigger over small period
    // Everything within explosion radius takes damage
    // If time make it so further from explosion radius
    // The less damage taken
    
    
    private Grid _gridReference;
    private GameObject _player;
    public enum SelectionOption
    {
        RoamBounds,
        FollowPatrolPoints
    }
    
    public enum EnemyType
    {
        Melee,
        Archer,
        Exploder
    }
   
    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Melee;
    
    [Header("Agent Type")]
    public SelectionOption selectionOption = SelectionOption.RoamBounds;
    
    //[Header("Positions")]
    private Transform _startPosition;
    private Transform _targetPosition;
    
    [Space(10)]
    
    [Header("Agent Behaviour")]
    public bool isFollowingPlayer;
    
    [Space(10)]
    
    [Header("Patrol Points")]
    public bool isTargetingPatrolPoints;
    public List<Transform> patrolPointsList;
    
    // Need organising
    // Difference must be always at least 3, any less causes issues
    public bool isAttacking;
    public float attackRange = 1.5f;
    public float aggroRange = 5f;
    
    //Patrol point stuff
    private int _currentWaypointIndex = 0;
    private bool _isMovingForward;
    
    // Getting the enemy to move initially
    private bool _initialPathCalculated = false;
    
    // Pathfinding optimisation
    private Vector3 _lastPlayerPosition;
    
    // Cannot pathfind if in attack range
    public bool canPathFindToTarget = true;
    
    private Vector3 _lastPosition;
    
    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _gridReference = FindObjectOfType<Grid>();
        _targetPosition = GameObject.FindWithTag("DefaultTargeting").transform;
    }

    private void Start()
    {
        if (selectionOption == SelectionOption.FollowPatrolPoints)
        {
            isTargetingPatrolPoints = true;
        }
        else
        {
            isTargetingPatrolPoints = false;
        }
        
        _startPosition = transform;
        
        // Some other optimisation thing
        //InvokeRepeating(nameof(CheckToFollowPlayer), 0f, 0.5f);
    }

    private void Update()
    { 
        CheckToFollowPlayer();
        Rotation();
    }

    private void Rotation()
    {
        if (!isFollowingPlayer)
        {
            // Calculate the direction vector of movement
            Vector3 direction = (transform.position - _lastPosition).normalized;

            var rotationSpeed = 5f;
            
            // Check if the enemy is moving (magnitude of direction is not close to zero)
            if (direction.magnitude > 0.01f)
            {
                // Rotate the enemy to face the direction of movement
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            // Update lastPosition to the current position for the next frame
            _lastPosition = transform.position;
        }
        
        if (isFollowingPlayer && _player != null)
        {
            // Look at the player if following
            Vector3 lookAtPosition =
                new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.LookAt(lookAtPosition);
        }
    }
    
    private void CheckToFollowPlayer()
    {
        if (_player == null)
        {
            return;
        }
        
        if (Vector3.Distance(transform.position, _player.transform.position) <= aggroRange)
        {
            isFollowingPlayer = true;
            if (_initialPathCalculated == false)
            {
                isFollowingPlayer = true;
                _targetPosition.position = _player.transform.position;
                _lastPlayerPosition = _player.transform.position;
                RecalculatePath();
                _initialPathCalculated = true;
            }
            else if (_initialPathCalculated)
            {
                if (Vector3.Distance(_lastPlayerPosition, _player.transform.position) >= attackRange)
                {
                    canPathFindToTarget = true;
                    _lastPlayerPosition = _player.transform.position;
                    _targetPosition.position = _lastPlayerPosition;
                    RecalculatePath();
                }
                else if (Vector3.Distance(_lastPlayerPosition, transform.position) <= attackRange)
                {
                    Debug.Log("In attack range");
                    canPathFindToTarget = false;
                    if (!isAttacking)
                    {
                        Attack();
                    }
                    _startPosition = transform;
                }
            }
        }
        else
        {
            if (isFollowingPlayer && !isTargetingPatrolPoints)
            {
                ChooseNewTarget();
            }

            else if (isFollowingPlayer && isTargetingPatrolPoints)
            {
                PatrolPointFollower();
            }

            isFollowingPlayer = false;
        }
    }

    private void Attack()
    {
        if (enemyType == EnemyType.Melee)
        {
            Melee?.Invoke(gameObject);
        }
        else if (enemyType == EnemyType.Archer)
        {
            Ranged?.Invoke(gameObject);
        }
        else if (enemyType == EnemyType.Exploder)
        {
            Explode?.Invoke(gameObject);
        }
    }
    
    private void RecalculatePath()
    {
        FindPath(_startPosition.position, _targetPosition.position);
    }
    
    private void FindPath(Vector3 aStartPos, Vector3 aTargetPos)
    {
        Node startNode = _gridReference.NodeFromWorldPoint(aStartPos);
        Node targetNode = _gridReference.NodeFromWorldPoint(aTargetPos);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                //If the f cost of that object is less than or equal to the f cost of the current node
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].IhCost < currentNode.IhCost)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                GetFinalPath(startNode, targetNode);
            }

            foreach (Node neighbourNode in _gridReference.GetNeighbouringNodes(currentNode))
            {
                if (!neighbourNode.IsWall || closedList.Contains(neighbourNode))
                {
                    continue;
                }
                
                //Get the F cost of that neighbor
                int moveCost = currentNode.IgCost + GetManhattenDistance(currentNode, neighbourNode);

                //If the f cost is greater than the g cost or it is not in the open list
                if (moveCost < neighbourNode.IgCost || !openList.Contains(neighbourNode))
                {
                    neighbourNode.IgCost = moveCost;
                    neighbourNode.IhCost = GetManhattenDistance(neighbourNode, targetNode);
                    neighbourNode.ParentNode = currentNode;

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
    }
    
    private void GetFinalPath(Node aStartingNode, Node aEndNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = aEndNode;

        _gridReference.SetEnemyFinalPath(gameObject, finalPath);
        
        while (currentNode != aStartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        finalPath.Reverse();

        _gridReference.FinalPath = finalPath;//Set the final path
    }
    
    public List<Node> GetFinalPath(GameObject enemy)
    {
        var enemyFinalPaths = _gridReference.enemyFinalPaths;
        
        if (enemyFinalPaths.ContainsKey(enemy))
        {
            return enemyFinalPaths[enemy];
        }
        else
        {
            Debug.LogWarning("No final path found for the specified enemy.");
            return null;
        }
    }

    int GetManhattenDistance(Node aNodeA, Node aNodeB)
    {
        int ix = Mathf.Abs(aNodeA.GridX - aNodeB.GridX);
        int iy = Mathf.Abs(aNodeA.GridY - aNodeB.GridY);

        return ix + iy;
    }
    
    public void ChooseNewTarget()
    {
        var gridX = (_gridReference.vGridWorldSize.x / _gridReference.fNodeDiameter) / 2;
        var gridY = (_gridReference.vGridWorldSize.y / _gridReference.fNodeDiameter) / 2;
        
        _targetPosition.position = new Vector3(Random.Range(-gridX,gridX), 0,
                                            Random.Range(-gridY,gridY));
        RecalculatePath();
    }
    
    public void PatrolPointFollower()
    {
        if (patrolPointsList.Count == 0)
        {
            Debug.LogWarning("Patrol Points not set!");
            return;
        }

        // Set the target position based on the current patrol index
        _targetPosition.position = patrolPointsList[_currentWaypointIndex].transform.position;
        RecalculatePath();

        // Move to the next patrol or the previous one if reached the end
        if (_isMovingForward)
        {
            // Increment the current patrol index
            _currentWaypointIndex++;

            // If reached the last patrol, start moving backward
            if (_currentWaypointIndex >= patrolPointsList.Count)
            {
                _currentWaypointIndex = patrolPointsList.Count - 2; // Set index to second-to-last patrol
                _isMovingForward = false;
            }
        }
        else
        {
            // Decrement the current patrol index
            _currentWaypointIndex--;

            // If reached the first patrol, start moving forward
            if (_currentWaypointIndex < 0)
            {
                _currentWaypointIndex = 1; // Set index to second patrol
                _isMovingForward = true;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw a wire sphere representing the sphere cast
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
}


