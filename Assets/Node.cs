using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
[Serializable]
public class Node {

    public int GridX;
    public int GridY;

    public bool IsWall;
    public Vector3 NodePosition;

    public Node ParentNode;

    public int IgCost;
    public int IhCost;

    public int FCost { get { return IgCost + IhCost; } }

    public Node(bool aIsWall, Vector3 aNPos, int gridX, int gridY)
    {
        IsWall = aIsWall;
        NodePosition = aNPos;
        GridX = gridX;
        GridY = gridY;
    }
}
    
}

