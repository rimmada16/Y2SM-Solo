using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Transform StartPosition;
    public LayerMask WallMask;
    public Vector2 vGridWorldSize;
    public float fNodeRadius;
    public float fDistanceBetweenNodes;

    Node[,] NodeArray;
    public List<Node> FinalPath;


    public float fNodeDiameter;
    int iGridSizeX, iGridSizeY;


    private void Start()
    {
        fNodeDiameter = fNodeRadius * 2;
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);
                bool Wall = !Physics.CheckSphere(worldPoint, fNodeRadius, WallMask);
                NodeArray[x, y] = new Node(Wall, worldPoint, x, y);
            }
        }
    }

    //Gets the neighbouring nodes of the given node.
    public List<Node> GetNeighbouringNodes(Node a_NeighbourNode)
    {
        List<Node> NeighbourList = new List<Node>();//Make a new list of all available neighbors.
        int icheckX;//Variable to check if the XPosition is within range of the node array to avoid out of range errors.
        int icheckY;//Variable to check if the YPosition is within range of the node array to avoid out of range errors.

        //Check the right side of the current node.
        icheckX = a_NeighbourNode.GridX + 1;
        icheckY = a_NeighbourNode.GridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)//If the YPosition is in range of the array
            {
                NeighbourList.Add(NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Left side of the current node.
        icheckX = a_NeighbourNode.GridX - 1;
        icheckY = a_NeighbourNode.GridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)//If the YPosition is in range of the array
            {
                NeighbourList.Add(NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Top side of the current node.
        icheckX = a_NeighbourNode.GridX;
        icheckY = a_NeighbourNode.GridY + 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)//If the YPosition is in range of the array
            {
                NeighbourList.Add(NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Bottom side of the current node.
        icheckX = a_NeighbourNode.GridX;
        icheckY = a_NeighbourNode.GridY - 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)//If the YPosition is in range of the array
            {
                NeighbourList.Add(NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }

        return NeighbourList;//Return the neighbors list.
    }

    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }
    
    public List<Node> GetFinalPath()
    {
        return FinalPath;
    }

    
    //Function that draws the wireframe
    private void OnDrawGizmos()
    {

        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));//Draw a wire cube with the given dimensions from the Unity inspector

        if (NodeArray != null)//If the grid is not empty
        {
            foreach (Node n in NodeArray)//Loop through every node in the grid
            {
                if (n.IsWall)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.yellow;//Set the color of the node
                }


                if (FinalPath != null)//If the final path is not empty
                {
                    if (FinalPath.Contains(n))//If the current node is in the final path
                    {
                        Gizmos.color = Color.red;//Set the color of that node
                    }

                }


                Gizmos.DrawCube(n.NodePosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.
            }
        }
    }
}


