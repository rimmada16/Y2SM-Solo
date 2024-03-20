using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour {

    Grid _gridReference;
    public Transform startPosition;
    public Transform targetPosition;
    private GameObject _player;
    
    
    private void Awake()
    {
        _gridReference = FindObjectOfType<Grid>();
        _player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        startPosition = transform;
    }

    private void Update()
    {
        if (_player != null)
        {
            targetPosition = _player.transform;  
        }
        FindPath(startPosition.position, targetPosition.position);
    }

    void FindPath(Vector3 aStartPos, Vector3 aTargetPos)
    {
        Node startNode = _gridReference.NodeFromWorldPoint(aStartPos);
        Node targetNode = _gridReference.NodeFromWorldPoint(aTargetPos);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Node currentNode = openList[0];
            for(int i = 1; i < openList.Count; i++)
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
    
    void GetFinalPath(Node aStartingNode, Node aEndNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = aEndNode;

        while(currentNode != aStartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        finalPath.Reverse();

        _gridReference.FinalPath = finalPath;//Set the final path

    }

    int GetManhattenDistance(Node aNodeA, Node aNodeB)
    {
        int ix = Mathf.Abs(aNodeA.GridX - aNodeB.GridX);
        int iy = Mathf.Abs(aNodeA.GridY - aNodeB.GridY);

        return ix + iy;
    }
}


