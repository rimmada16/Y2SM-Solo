using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pathfinding : MonoBehaviour {

    private Grid _gridReference;
    private GameObject _player;
    public enum SelectionOption
    {
        RoamBounds,
        FollowWaypoints
    }
    [Header("Agent Type")]
    public SelectionOption selectionOption = SelectionOption.RoamBounds;
    
    [Space(10)]
    
    [Header("Positions")]
    public Transform startPosition;
    public Transform targetPosition;
    
    [Space(10)]
    
    [Header("Agent Behaviour")]
    public bool isFollowingPlayer;
    
    [Space(10)]
    
    [Header("Waypoints")]
    public bool isTargetingWaypoints;
    [SerializeField] private List<Transform> waypointsList;

    [Space(10)]
    
    [SerializeField] private Collider[] results;
    
    private bool _isAttacking = false;
    private bool waitToCalculatePath = false;
    private float attackRange = 2f;
    private float aggroRange = 2.5f;
    private int currentWaypointIndex = 0;
    private bool isMovingForward;
    bool initialPathCalculated = false;
    Vector3 lastPlayerPosition;
    private bool firstTime;
    public bool canPathfind = true;
    private int currentNodeIndex;
    private Animation anim;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _gridReference = FindObjectOfType<Grid>();
        //results = new Collider[1];
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animation>();
        if (selectionOption == SelectionOption.FollowWaypoints)
        {
            isTargetingWaypoints = true;
        }
        else
        {
            isTargetingWaypoints = false;
        }
        
        startPosition = transform;
    }

    private void Update()
    {
        //DoSphereCast();

        CheckToFollowPlayer();
        Rotation();
    }

    private void Rotation()
    {
        if (!isFollowingPlayer)
        {
            Vector3 lookAtPosition =
                new Vector3(targetPosition.position.x, transform.position.y, targetPosition.position.z);
            transform.LookAt(lookAtPosition);
        }
        else if (isFollowingPlayer)
        {
            // Look at the player if following
            Vector3 lookAtPosition =
                new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.LookAt(lookAtPosition);
        }
    }

    private void CheckToFollowPlayer()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= 5)
        {
            isFollowingPlayer = true;
            if (initialPathCalculated == false)
            {
                isFollowingPlayer = true;
                targetPosition.position = _player.transform.position;
                lastPlayerPosition = _player.transform.position;
                RecalculatePath();
                initialPathCalculated = true;
            }
            else if (initialPathCalculated)
            {
                if (Vector3.Distance(lastPlayerPosition, _player.transform.position) >= 2)
                {
                    canPathfind = true;
                    lastPlayerPosition = _player.transform.position;
                    targetPosition.position = lastPlayerPosition;
                    RecalculatePath();
                }
                else if (Vector3.Distance(lastPlayerPosition, transform.position) <= 2)
                {
                    Debug.Log("In attack range");
                    canPathfind = false;
                    if (!_isAttacking)
                    {
                        StartCoroutine(Attack());
                    }

                    //StopMoving();
                    startPosition = transform;
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

    private IEnumerator Attack()
    {
        _isAttacking = true;
        anim.Play();
        yield return new WaitForSeconds(1f);
        _isAttacking = false;
        yield return null;
    }
    
    private void RecalculatePath()
    {
        FindPath(startPosition.position, targetPosition.position);
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
        
        targetPosition.position = new Vector3(Random.Range(-gridX,gridX), 0,
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
        targetPosition.position = waypointsList[currentWaypointIndex].transform.position;
        RecalculatePath();

        // Move to the next waypoint or the previous one if reached the end
        if (isMovingForward)
        {
            // Increment the current waypoint index
            currentWaypointIndex++;

            // If reached the last waypoint, start moving backward
            if (currentWaypointIndex >= waypointsList.Count)
            {
                currentWaypointIndex = waypointsList.Count - 2; // Set index to second-to-last waypoint
                isMovingForward = false;
            }
        }
        else
        {
            // Decrement the current waypoint index
            currentWaypointIndex--;

            // If reached the first waypoint, start moving forward
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 1; // Set index to second waypoint
                isMovingForward = true;
            }
        }
    }

    
    private void OnDrawGizmos()
    {
        // Draw a wire sphere representing the sphere cast
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5f);
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


