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
    private Pathfinding _pathfinding;
    private bool _initialPathCalculated;
    private bool _canFollowPath = true;




    private void Awake()
    {
        _pathfinding = GetComponent<Pathfinding>();

        // Set the initial y-coordinate of the position to 1
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        
    }

    private void Update()
    {
        if (_canFollowPath && _pathfinding.canPathfind)
        {
            FollowPath();
        }

        // So that the enemy starts on a path when the game starts
        if (!_initialPathCalculated && !_pathfinding.isTargetingWaypoints)
        {
            _pathfinding.ChooseNewTarget();
            _initialPathCalculated = true;
        }
        // If waypoints are being targeted on start travel to the first waypoint
        else if (!_initialPathCalculated && _pathfinding.isTargetingWaypoints)
        {
            _pathfinding.WaypointFollower();
            _initialPathCalculated = true;
        }
    }

    private void FollowPath()
    {
        List<Node> finalPath = _pathfinding.GetFinalPath(gameObject);

        if (finalPath == null || finalPath.Count == 0)
        {
            // Reset _currentNodeIndex and return
            _currentNodeIndex = 0;
            return;
        }

        // Ensure _currentNodeIndex is within the valid range of indices
        if (_currentNodeIndex < 0 || _currentNodeIndex >= finalPath.Count)
        {
            // Reset _currentNodeIndex and return
            _currentNodeIndex = 0;
            return;
        }
    
        // Move towards the current node
        transform.position = Vector3.MoveTowards(transform.position, finalPath[_currentNodeIndex].NodePosition, movementSpeed * Time.deltaTime);

        // Check if the enemy's position matches the position of the current node
        if (Vector3.Distance(transform.position, finalPath[_currentNodeIndex].NodePosition) < .1f)
        {
            // Increment currentNodeIndex
            _currentNodeIndex++;

            // Check if reached the end of the path
            if (_currentNodeIndex >= finalPath.Count)
            {
                Debug.Log("Reached the end of the path!");
            
                // Reset _currentNodeIndex
                _currentNodeIndex = 0;
            
                // Find a new target and make the enemy wait before following the path

                if (_pathfinding.isTargetingWaypoints && !_pathfinding.isFollowingPlayer)
                {
                    _pathfinding.WaypointFollower();
                }

                else if (!_pathfinding.isFollowingPlayer)
                {
                    _pathfinding.ChooseNewTarget();
                }
                
                _canFollowPath = false;
                StartCoroutine(WaitBeforeFollowingPath());
            }
        }
    }
    
    private IEnumerator WaitBeforeFollowingPath()
    {
        yield return new WaitForSeconds(1f);
        _canFollowPath = true;
    }
}