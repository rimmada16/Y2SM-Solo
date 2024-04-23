using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    /// <summary>
    /// Handles the movement of the enemy agents
    /// </summary>
    public class EnemyMovement : MonoBehaviour
    {
        public float movementSpeed = 5f; // Adjust the speed as needed
        private int _currentNodeIndex = 0;
        private Pathfinding _pathfinding;
        private bool _initialPathCalculated;
        private bool _canFollowPath = true;

        /// <summary>
        /// Get the Pathfinding component
        /// </summary>
        private void Awake()
        {
            _pathfinding = GetComponent<Pathfinding>();
        }

        /// <summary>
        /// Check if the enemy can follow the path and follow the path
        /// </summary>
        private void Update()
        {
            if (_canFollowPath && _pathfinding.canPathFindToTarget)
            {
                FollowPath();
            }

            // So that the enemy starts on a path when the game starts
            if (!_initialPathCalculated && !_pathfinding.isTargetingPatrolPoints)
            {
                _pathfinding.ChooseNewTarget();
                _initialPathCalculated = true;
            }
            // If waypoints are being targeted on start travel to the first waypoint
            else if (!_initialPathCalculated && _pathfinding.isTargetingPatrolPoints)
            {
                _pathfinding.PatrolPointFollower();
                _initialPathCalculated = true;
            }
        }
        
        /// <summary>
        /// Retrieves the path to follow from the pathfinding script. If certain conditions are met, resets the node
        /// index. Moves the enemy towards each node sequentially, incrementing the node index upon reaching each node.
        /// When the node index equals the total number of nodes in the path, it resets the node index. If the enemy is
        /// patrolling, calls the PatrolPointFollower method from the pathfinding script. If the enemy is not
        /// patrolling, obtains a new target and initiates a coroutine to introduce a delay before continuing onto the
        /// new path.
        /// </summary>
        private void FollowPath()
        {
            List<Node> finalPath = _pathfinding.GetFinalPath(gameObject);

            if (finalPath == null || finalPath.Count == 0 || _currentNodeIndex < 0 || _currentNodeIndex >= finalPath.Count)
            {
                // Reset _currentNodeIndex and return
                _currentNodeIndex = 0;
                return;
            }

            // Move towards the current node
            transform.position = Vector3.MoveTowards(transform.position, finalPath[_currentNodeIndex].nodePosition, movementSpeed * Time.deltaTime);

            // Check if the enemy's position matches the position of the current node
            if (Vector3.Distance(transform.position, finalPath[_currentNodeIndex].nodePosition) < .1f)
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

                    if (_pathfinding.isTargetingPatrolPoints && !_pathfinding.isFollowingPlayer)
                    {
                        _pathfinding.PatrolPointFollower();
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

        /// <summary>
        /// Coroutine to introduce a delay before resuming following the path.
        /// </summary>
        private IEnumerator WaitBeforeFollowingPath()
        {
            yield return new WaitForSeconds(1f);
            _canFollowPath = true;
        }
    }
}
