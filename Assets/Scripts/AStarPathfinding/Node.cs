using System;
using UnityEngine;

namespace AStarPathfinding
{
    /// <summary>
    /// A node in the grid used for pathfinding
    /// </summary>
    [Serializable]
    public class Node 
    {
        /// <summary>
        /// X grid position
        /// </summary>
        public int gridX;
        
        /// <summary>
        /// Y grid position
        /// </summary>
        public int gridY;

        /// <summary>
        /// Is the node a wall
        /// </summary>
        public bool isWall;
        
        /// <summary>
        /// Node position in world space - XYZ
        /// </summary>
        public Vector3 nodePosition;

        /// <summary>
        /// The parent node
        /// </summary>
        public Node parentNode;

        // Cost values for pathfinding (gCost: cost from start, hCost: heuristic cost to end)
        /// <summary>
        /// Cost from the start
        /// </summary>
        public int gCost;
        
        /// <summary>
        /// Heuristic cost to end
        /// </summary>
        public int hCost;
        
        /// <summary>
        /// Total cost of the node (sum of gCost and hCost)
        /// </summary>
        public int FCost => gCost + hCost;

        /// <summary>
        /// Constructor for creating a node.
        /// </summary>
        /// <param name="isWall">Whether the node is a wall (obstacle) or not.</param>
        /// <param name="nodePosition">Position of the node in world space.</param>
        /// <param name="gridX">X-coordinate of the node in the grid.</param>
        /// <param name="gridY">Y-coordinate of the node in the grid.</param>
        public Node(bool isWall, Vector3 nodePosition, int gridX, int gridY)
        {
            this.isWall = isWall;
            this.nodePosition = nodePosition;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}

