using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed = 5f; // Adjust the speed as needed
    private int _currentNodeIndex = 0;
    private Grid _grid;

    private void Awake()
    {
        _grid = FindObjectOfType<Grid>(); // Find the Grid script in the scene

        // Set the initial y-coordinate of the position to 1
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        
    }

    private void Update()
    {
        FollowPath();

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("current n index "+_currentNodeIndex);
        }
    }

    private void FollowPath()
    {
        List<Node> finalPath = _grid.GetFinalPath();
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("finalpath " + finalPath.Count);
        }
        
        if (finalPath == null || finalPath.Count == 0 || _currentNodeIndex >= finalPath.Count)
        {
            return; 
        }
        
        

        // Move towards the current node
        transform.position = Vector3.MoveTowards(transform.position, finalPath[_currentNodeIndex].NodePosition, movementSpeed * Time.deltaTime);
        
        
        // This stupid if statement never runs, could be due to tolerance idk

        // Check if the enemy's position matches the position of the current node
        if (Vector3.Distance(transform.position, finalPath[_currentNodeIndex].NodePosition) < .1f)
        {
            Debug.Log("Reached node position: " + finalPath[_currentNodeIndex].NodePosition);

            // Increment currentNodeIndex
            _currentNodeIndex++;

            // Check if reached the end of the path
            if (_currentNodeIndex >= finalPath.Count)
            {
                Debug.Log("Reached the target!");
                // Handle reaching the target here
            }
        }

        // Ensure the y-coordinate remains at 1 unit
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }



}