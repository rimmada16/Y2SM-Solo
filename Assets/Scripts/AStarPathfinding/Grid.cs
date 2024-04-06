using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AStarPathfinding
{
/// <summary>
///Represents a grid used for pathfinding in the game environment. This class manages the creation of a grid layout,
/// defines obstacles, and provides methods to retrieve neighboring nodes and find nodes based on world positions.
/// It also visualizes the grid and node information in the Scene view for debugging purposes.
/// </summary> 
public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2 gridWorldSize;
    private int _gridSizeX, _gridSizeY;
    
    [Header("What is an Obstacle?")]
    [SerializeField] private LayerMask wallMask;
    
    [Header("Node Settings")]
    [SerializeField] private float nodeRadius;
    [SerializeField] private float distanceBetweenNodes;
    public float nodeDiameter;
    private Node[,] _nodeArray;
    public readonly Dictionary<GameObject, List<Node>> EnemyFinalPaths = new();

    /// <summary>
    /// Grid initialization via calculation of needed parameters and then actually creates the grid.
    /// </summary>
    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    
    /// <summary>
    /// Creates the grid for the pathfinding algorithm using the specified parameters.
    /// </summary>
    private void CreateGrid()
    {
        _nodeArray = new Node[_gridSizeX, _gridSizeY];
        var bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (var x = 0; x < _gridSizeX; x++)
        {
            for (var y = 0; y < _gridSizeY; y++)
            {
                var worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                var wall = !Physics.CheckSphere(worldPoint, nodeRadius, wallMask);
                _nodeArray[x, y] = new Node(wall, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// Gets the neighboring nodes of the given node.
    /// </summary>
    /// <param name="neighbourNode">The node whose neighbors are to be obtained.</param>
    /// <returns>A list containing all neighboring nodes of the specified node.</returns>
    public List<Node> GetNeighbouringNodes(Node neighbourNode)
    {
        // Make a new list of all available neighbors
        var neighbourList = new List<Node>();

        for (var x = -1; x <= 1; x++) 
        {
            for (var y = -1; y <= 1; y++) 
            {
                // If we are on the node that was passed in, skip this iteration.
                if (x == 0 && y == 0) 
                {
                    continue;
                }

                var checkX = neighbourNode.gridX + x;
                var checkY = neighbourNode.gridY + y;

                // Make sure the node is within the grid.
                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY) 
                {
                    // Adds to the neighbours list.
                    neighbourList.Add(_nodeArray[checkX, checkY]); 
                }
            }
        }

        return neighbourList;
    }

    //Gets the closest node to the given world position.
    /// <summary>
    /// Returns the node corresponding to the specified world position.
    /// </summary>
    /// <param name="worldPos">The world position use to find the closest node.</param>
    /// <returns>The node corresponding to the specified world position.</returns>
    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        var xPos = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        var yPos = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        xPos = Mathf.Clamp01(xPos);
        yPos = Mathf.Clamp01(yPos);

        var x = Mathf.RoundToInt((_gridSizeX - 1) * xPos);
        var y = Mathf.RoundToInt((_gridSizeY - 1) * yPos);

        return _nodeArray[x, y];
    }
    
    /// <summary>
    /// Sets the final path for the specified enemy GameObject.
    /// </summary>
    /// <param name="enemy">The GameObject representing the enemy.</param>
    /// <param name="theFinalPath">The final path of nodes for the enemy.</param>
    public void SetEnemyFinalPath(GameObject enemy, List<Node> theFinalPath)
    {
        EnemyFinalPaths[enemy] = theFinalPath;
    }
    
    /// <summary>
    /// Draws Gizmos in the Scene view for visualizing the grid and node information.
    /// Grid will be in a grey colour, obstacles are visualised in yellow and the final path of the enemy is in red.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));//Draw a wire cube with the given dimensions from the Unity inspector

        if (_nodeArray == null) 
        {
            return; //If the grid is not empty
        }
        
        foreach (Node n in _nodeArray)//Loop through every node in the grid
        {
            if (n.isWall)//If the current node is a wall node
            {
                Gizmos.color = Color.white;//Set the color of the node
            }
            else
            {
                Gizmos.color = Color.yellow;//Set the color of the node
            }

            // Check if any enemy final paths exist and if the current node is in any of them
            foreach (var kvp in EnemyFinalPaths)
            {
                if (kvp.Value.Contains(n))//If the current node is in the final path of the enemy
                {
                    Gizmos.color = Color.red;//Set the color of that node
                    break; // Exit the loop if the node is found in any enemy's final path
                }
            }
            Gizmos.DrawCube(n.nodePosition, Vector3.one * (nodeDiameter - distanceBetweenNodes)); //Draw the node at the position of the node.
        }
    }
}
}
