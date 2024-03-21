using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed = 5f; // Adjust the speed as needed
    private int _currentNodeIndex = 0;
    private Grid _grid;
    private Pathfinding _pathfinding;
    private bool funnyBool;

    private void Awake()
    {
        _pathfinding = GetComponent<Pathfinding>();
        _grid = FindObjectOfType<Grid>(); // Find the Grid script in the scene

        // Set the initial y-coordinate of the position to 1
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        
    }

    private void Start()
    {
        _pathfinding.RecalculatePath();
    }
    
    private void Update()
    {
        FollowPath();
        
        if (_currentNodeIndex == 0 && !funnyBool)
        {
            ChooseNewTarget();
            funnyBool = true;
        }
    }

    private void FollowPath()
    {
       // funnyBool = true;
        List<Node> finalPath = _grid.GetFinalPath(gameObject);
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("finalpath " + finalPath.Count);
        }
        
        if (finalPath == null || finalPath.Count == 0)
        {
            return; 
        }
        
        

        // Move towards the current node
        transform.position = Vector3.MoveTowards(transform.position, finalPath[_currentNodeIndex].NodePosition, movementSpeed * Time.deltaTime);

        // Check if the enemy's position matches the position of the current node
        if (Vector3.Distance(transform.position, finalPath[_currentNodeIndex].NodePosition) < .1f)
        {
            //Debug.LogWarning("Reached node position: " + finalPath[_currentNodeIndex].NodePosition);

            // Increment currentNodeIndex
            _currentNodeIndex++;

            // Check if reached the end of the path
            if (_currentNodeIndex >= finalPath.Count -1)
            {
                Debug.Log("Reached the target!");
                // Handle reaching the target here

                ChooseNewTarget();
            }
        }

        // Ensure the y-coordinate remains at 1 unit
        // transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }

    private void ChooseNewTarget()
    {
        var gridX = (_grid.vGridWorldSize.x / _grid.fNodeDiameter) / 2;
        var gridY = (_grid.vGridWorldSize.y / _grid.fNodeDiameter) / 2;
        
        _pathfinding.targetPosition.position = new Vector3(Random.Range(-gridX,gridX), 0,
            Random.Range(-gridY,gridY));
        _currentNodeIndex = 0;
        _pathfinding.RecalculatePath();
    }
}