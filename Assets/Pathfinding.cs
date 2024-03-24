using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pathfinding : MonoBehaviour {

    private Grid _gridReference;
    public Transform startPosition;
    public Transform targetPosition;
    private GameObject _player;
    public bool isFollowingPlayer;
    private bool isAttacking = false;
    private float attackRange = 2f;
    private float aggroRange = 2.5f;
    public bool cock;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _gridReference = FindObjectOfType<Grid>();
    }

    private void Start()
    {
        startPosition = transform;
    }

    private void Update()
    {
        DoSphereCast();
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

    
    private void DoSphereCast()
    {
        float sphereCastRadius = 5f;
        float maxDistance = 4f;

        LayerMask playerLayer = LayerMask.GetMask("Player");

        // Calculate the direction from the agent to the player
        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        
        RaycastHit hit;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereCastRadius, playerLayer);

        // Check if any colliders were found
        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider collider1 in colliders)
            {
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

                if (distanceToPlayer <= 2f)
                {
                    // Add is attacking to check, gotta be false at start
                    
                    StopMoving();
                    // Start attack coroutine which starts an anim
                    // Co makes above bool true at start and false at end
                }

                if (distanceToPlayer > 2.1f && isFollowingPlayer)
                {
                    targetPosition.position = _player.transform.position;
                    //StartCoroutine(WaitToCalculatePath());
                    
                    
                    // EXPENSIVE VERY VERY EXPENSIVE
                    
                    RecalculatePath();
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
        } 
        
        
        
        /*if (Physics.SphereCast(origin, 5f, direction, out hit, maxDistance, 9))
        {
            isFollowingPlayer = true;
            Debug.Log("Supposedly hit the player!");
            targetPosition.position = _player.transform.position;
            RecalculatePath();
            Debug.DrawLine(origin, hit.point, Color.red);
        }
        else
        {
            isFollowingPlayer = false;
        }

        if (distanceToPlayer <= 1f)
        {
            StopMoving();
        }

        if (distanceToPlayer > 1f && isFollowingPlayer)
        {
            targetPosition.position = _player.transform.position;
            RecalculatePath();
        }*/
    }



    public void StopMoving()
    {
        // Stop any ongoing movement or pathfinding
        targetPosition.position = transform.position;
        RecalculatePath(); // Ensure that the current path is cleared
    }
    
    private void OnDrawGizmos()
    {
        // Draw a wire sphere representing the sphere cast
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }


    
}


