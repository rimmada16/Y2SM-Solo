using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pathfinding : MonoBehaviour {

    public static event ExploderEvent Explode;
    
    
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
        FollowWaypoints
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
    
    //[Space(10)]
    
    //[Header("Positions")]
    private Transform _startPosition;
    private Transform _targetPosition;
    
    [Space(10)]
    
    [Header("Agent Behaviour")]
    public bool isFollowingPlayer;
    
    [Space(10)]
    
    [Header("Waypoints")]
    public bool isTargetingWaypoints;
    public List<Transform> waypointsList;

    [Space(10)]
    
    [SerializeField] private Collider[] results;

    
    // Need organising
    private bool _isAttacking;
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
    public bool canPathfind = true;
    
    // For the sword
    private Animation _anim;
    
    // How long enemy has to wait before attacking again - if melee
    public float attackFrequency = 2f;

    // Arrow type to feed into Projectile Manager
    public int arrowToShoot;


    private Vector3 _lastPosition;
    
    // XML WHERE!?!?!?!?
    
    
    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _gridReference = FindObjectOfType<Grid>();
        _targetPosition = GameObject.FindWithTag("DefaultTargeting").transform;
        //results = new Collider[1];
    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animation>();
        if (selectionOption == SelectionOption.FollowWaypoints)
        {
            isTargetingWaypoints = true;
        }
        else
        {
            isTargetingWaypoints = false;
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
        // if (!isFollowingPlayer)
        // {
        //     Vector3 lookAtPosition =
        //         new Vector3(_targetPosition.position.x, transform.position.y, _targetPosition.position.z);
        //     transform.LookAt(lookAtPosition);
        // }

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
        // PULLED YESTERDAYS CODE OF GIT
        // WORKS AND IS INSANELY OPTIMISED COMPARED TO
        // WHAT WAS WRITTEN TODAY
        // !?!?!?!?!?!?!??!?!?!?!?!?!??!?!?!?!?!?!??!

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
                    canPathfind = true;
                    _lastPlayerPosition = _player.transform.position;
                    _targetPosition.position = _lastPlayerPosition;
                    RecalculatePath();
                }
                else if (Vector3.Distance(_lastPlayerPosition, transform.position) <= attackRange)
                {
                    // SHOULD CALL A METHOD IN A DIFF CLASS - LOGIC SHOULD NOT BE HERE
                    // COROUTINE SHOULD BE MOVED AS WELL
                    
                    Debug.Log("In attack range");
                    canPathfind = false;
                    if (!_isAttacking && enemyType == EnemyType.Melee)
                    {
                        // Coroutine for the melee attack
                        StartCoroutine(MeleeAttack());
                    }
                    else if (!_isAttacking && enemyType == EnemyType.Archer)
                    {
                        StartCoroutine(BowAttack());
                    }
                    else if (!_isAttacking && enemyType == EnemyType.Exploder)
                    {
                        Explode?.Invoke(gameObject);
                    }

                    //StopMoving();
                    _startPosition = transform;
                }
            }
        }
        else
        {
            if (isFollowingPlayer && !isTargetingWaypoints)
            {
                ChooseNewTarget();
            }

            else if (isFollowingPlayer && isTargetingWaypoints)
            {
                WaypointFollower();
            }

            isFollowingPlayer = false;
        }
    }

    
    
    /*private void CheckToFaollowPlayer()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
    
        if (distanceToPlayer > aggroRange)
        {
            _initialPathCalculated = false;
        }
        
        
        if (distanceToPlayer <= aggroRange)
        {
            isFollowingPlayer = true;
            if (_initialPathCalculated == false)
            {
                _targetPosition.position = _player.transform.position;
                RecalculatePath();
                _initialPathCalculated = true;
            }
            else if (_initialPathCalculated && reachedPointAfterInitialCalc)
            {
                if (distanceToPlayer >= 3f)
                {
                    if (setPos)
                    {
                        _targetPosition.position = _player.transform.position;
                        RecalculatePath();
                        setPos = false;
                    }
                }
                
                
                
                
                
                // If the player has moved further than x units, recalculate the path
                // Settled on 3 during optimisation tests as it seemed to be the best balance
                /*if (distanceToPlayer >= 3f)
                {
                    canPathfind = true;
                    _lastPlayerPosition = _player.transform.position;
                    _targetPosition.position = _lastPlayerPosition;
                    _initialPathCalculated = false;
                    // if (Time.time - lastPathfindingTime < timeBetweenPathfinding)
                    // {
                    //     return;
                    // }
                    
                    //lastPathfindingTime = Time.time;
                    //RecalculatePath();
                }#1#
                if (distanceToPlayer <= attackRange)
                {
                    //Debug.Log("In attack range");
                    setPos = true;
                    canPathfind = false;
                    if (!_isAttacking)
                    {
                        StartCoroutine(Attack());
                    }
                }
                // else if (_justAttacked && distanceToPlayer <= 3f)
                // {
                //     canPathfind = true;
                //     _lastPlayerPosition = _player.transform.position;
                //     _targetPosition.position = _lastPlayerPosition;
                //     RecalculatePath();
                //     _justAttacked = false;
                // }
            }
        }
        else
        {
            if (isFollowingPlayer && !isTargetingWaypoints)
            {
                ChooseNewTarget();
            }
            else if (isFollowingPlayer && isTargetingWaypoints)
            {
                WaypointFollower();
            }
            
            isFollowingPlayer = false;
        }
    }*/

    private IEnumerator MeleeAttack()
    {
        _isAttacking = true;
        _anim.Play();
        // Wait amount should be dependant on the weapon type used.
        // Currently is not
        yield return new WaitForSeconds(attackFrequency);
        _isAttacking = false;
        
        yield return null;
    }

    private IEnumerator BowAttack()
    {
        _isAttacking = true;
        ProjectileManager.Instance.FireProjectile(arrowToShoot, transform.position, transform.forward, gameObject);
        yield return new WaitForSeconds(attackFrequency);
        _isAttacking = false;
        
        yield return null;
    }
    
    private IEnumerator ExploderAttack()
    {
        _isAttacking = true;
        // Logic for the exploder attack
        
        // Start a coroutine that calls an event
        // Exploder class subbed to this event
        // Send in the gameobject
        // if gameobject = that gameobject, explode
        
        yield return new WaitForSeconds(attackFrequency);
        _isAttacking = false;
        
        yield return null;
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
    
    public void WaypointFollower()
    {
        if (waypointsList.Count == 0)
        {
            Debug.LogWarning("Waypoints not set!");
            return;
        }

        // Set the target position based on the current waypoint index
        _targetPosition.position = waypointsList[_currentWaypointIndex].transform.position;
        RecalculatePath();

        // Move to the next waypoint or the previous one if reached the end
        if (_isMovingForward)
        {
            // Increment the current waypoint index
            _currentWaypointIndex++;

            // If reached the last waypoint, start moving backward
            if (_currentWaypointIndex >= waypointsList.Count)
            {
                _currentWaypointIndex = waypointsList.Count - 2; // Set index to second-to-last waypoint
                _isMovingForward = false;
            }
        }
        else
        {
            // Decrement the current waypoint index
            _currentWaypointIndex--;

            // If reached the first waypoint, start moving forward
            if (_currentWaypointIndex < 0)
            {
                _currentWaypointIndex = 1; // Set index to second waypoint
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

    /*public void StopMoving()
    {
        canPathfind = false;
        // Stop any ongoing movement or pathfinding
        //targetPosition.position = transform.position;

        if (firstTime == false)
        {
            RecalculatePath(); // Ensure that the current path is cleared
            firstTime = true;   
        }
    }*/

    /*private void DoSphereCast()
    {
        float sphereCastRadius = 5f;
        float maxDistance = 4f;

        LayerMask playerLayer = LayerMask.GetMask("Player");
        
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        Array.Clear(results, 0, results.Length);
        int size = Physics.OverlapSphereNonAlloc(transform.position, sphereCastRadius, results, playerLayer);

        // Check if any colliders were found
        if (results != null && results.Length > 0)
        {
            for (int i = 0; i < size; i++)
            {
                Collider collider1 = results[i];
                
                //Debug.Log("Hit something: " + collider.name);
                if (collider1.gameObject.CompareTag("Player"))
                {
                    isFollowingPlayer = true;
                    Debug.Log("Supposedly hit the player!");
                    targetPosition.position = _player.transform.position;
                    RecalculatePath();
                }
                else
                {
                    isFollowingPlayer = false;
                }

                if (distanceToPlayer <= 2f && !_isAttacking)
                {
                    // Add is attacking to check, gotta be false at start

                    if (anim != null)
                    {
                        _isAttacking = true;
                        anim.Play();
                    }
                    else
                    {
                        Debug.LogError("No animation component found!");
                    }
                }

                if (distanceToPlayer > 2.1f && isFollowingPlayer)
                {
                    targetPosition.position = _player.transform.position;
                    if (waitToCalculatePath == false)
                    {
                        waitToCalculatePath = true;
                        StartCoroutine(WaitToCalculatePath());
                    }
                    
                    
                    
                    // EXPENSIVE VERY VERY EXPENSIVE
                    
                    //RecalculatePath();
                }
            }
        }

        if (distanceToPlayer > sphereCastRadius && isFollowingPlayer)
        {
            isFollowingPlayer = false;
        } 
        
        IEnumerator WaitToCalculatePath()
        {
            yield return new WaitForSeconds(1f);
            RecalculatePath();
            waitToCalculatePath = false;
        }
    }*/


