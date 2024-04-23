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
        public int gridX;
        public int gridY;

        public bool isWall;
        public Vector3 nodePosition;

        public Node parentNode;

        // Cost values for pathfinding (gCost: cost from start, hCost: heuristic cost to end)
        public int gCost;
        public int hCost;

        // Total cost of the node (sum of gCost and hCost)
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

