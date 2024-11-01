using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AStarPathfinding
{
    /// <summary>
    /// Handles the overall Pathfinding logic for the enemy agents.
    /// </summary>
    public class Pathfinding : MonoBehaviour
    {
        /// <summary>
        /// Event for triggering the ExploderUnit attack
        /// </summary>
        public static event ExploderEvent Explode;
        
        /// <summary>
        /// Event for triggering the MeleeUnit attack
        /// </summary>
        public static event MeleeAttackEvent Melee;
        
        /// <summary>
        /// Event for triggering the ArcherUnit attack
        /// </summary>
        public static event RangedAttackEvent Ranged;

        // For interaction with the scene
        private Grid _gridReference;
        private GameObject _player;

        /// <summary>
        /// Choose whether the agent is roaming or following patrol points
        /// </summary>
        public enum SelectionOption
        {
            RoamBounds,
            FollowPatrolPoints
        }

        /// <summary>
        /// The type of agent
        /// </summary>
        public enum EnemyType
        {
            Melee,
            Archer,
            Exploder
        }

        /// <summary>
        /// Selection option for the agent type - Melee, Archer, Exploder
        /// </summary>
        [Header("Enemy Type")] 
        public EnemyType enemyType = EnemyType.Melee;

        /// <summary>
        /// Selection option for the agent - Roaming or Patrol Points
        /// </summary>
        [Header("Agent Type")] 
        public SelectionOption selectionOption = SelectionOption.RoamBounds;
        
        /// <summary>
        /// Lets the pathfinding system know if the agent is following a player or not
        /// </summary>
        [Space(10)] [Header("Agent Behaviour")]
        public bool isFollowingPlayer;
        
        /// <summary>
        /// States whether the unit is attacking or not
        /// </summary>
        public bool isAttacking;
        
        /// <summary>
        /// Lets the agent know whether they can path to their intended target
        /// </summary>
        public bool canPathFindToTarget = true;

        /// <summary>
        /// Lets the system know if the agent is targeting patrol points
        /// </summary>
        [Space(10)] [Header("Patrol Points")]
        public bool isTargetingPatrolPoints;
        
        /// <summary>
        /// List of patrol points
        /// </summary>
        public List<Transform> patrolPointsList;
        
        private Transform _startPosition, _targetPosition;
        private int _currentPatrolPointIndex = 0;
        private bool _isMovingForward;

        // Difference must be always at least 3, any less causes issues
        /// <summary>
        /// The attack range of the Unit
        /// </summary>
        public float attackRange = 1.5f;
        
        /// <summary>
        /// The aggression range of the Unit
        /// </summary>
        public float aggroRange = 5f;

        // Getting the enemy to move initially
        private bool _initialPathCalculated;

        // Pathfinding optimisation
        private Vector3 _lastPlayerPosition, _lastPosition;
        
        /// <summary>
        /// List to display all the nodes in the path for the Unit in the Editor
        /// </summary>
        public List<Node> finalPathOfNodes;

        /// <summary>
        /// Fills in the player and grid references required for the script to function.
        /// Sets the initial target position to the default target position GameObject.
        /// </summary>
        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _gridReference = FindObjectOfType<Grid>();
            _targetPosition = GameObject.FindWithTag("DefaultTargeting").transform;
        }

        /// <summary>
        /// isTargetingPatrolPoints set to true or false depending on the selection option.
        /// start position is set to the enemies initial transform.
        /// </summary>
        private void Start()
        {
            isTargetingPatrolPoints = selectionOption == SelectionOption.FollowPatrolPoints;

            _startPosition = transform;
        }

        /// <summary>
        /// Calls the CheckToFollowPlayer method and the Rotation method.
        /// </summary>
        private void FixedUpdate()
        {
            CheckToFollowPlayer();
            Rotation();
            PlayerDeath();
        }

        private void PlayerDeath()
        {
            if (_player != null)
            {
                return;
            }

            if (isFollowingPlayer)
            {
                isFollowingPlayer = false;
                canPathFindToTarget = true;
            }
        }

        /// <summary>
        /// If the enemy is not following the player, rotate the enemy to face the direction of movement.
        /// If the enemy is following the player, rotate the enemy to face the player.
        /// </summary>
        private void Rotation()
        {
            if (!isFollowingPlayer)
            {
                // Calculate the direction vector of movement
                var direction = (transform.position - _lastPosition).normalized;
                var rotationSpeed = 5f;

                // Check if the enemy is moving (magnitude of direction is not close to zero)
                if (direction.magnitude > 0.01f)
                {
                    // Rotate the enemy to face the direction of movement
                    var targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                // Update lastPosition to the current position for the next frame
                _lastPosition = transform.position;
            }

            if (isFollowingPlayer && _player != null)
            {
                // Look at the player if following
                var playerPosition = _player.transform.position;
                var lookAtPosition =
                    new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                transform.LookAt(lookAtPosition);
            }
        }

        /// <summary>
        /// Controls the logic for the enemy to follow the player depending on the distance between the player and the enemy.
        /// </summary>
        private void CheckToFollowPlayer()
        {
            if (_player == null)
            {
                return;
            }

            var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
    
            if (distanceToPlayer <= aggroRange)
            {
                isFollowingPlayer = true;
                if (!_initialPathCalculated)
                {
                    StartFollowingPlayer();
                }
                else if (Vector3.Distance(_lastPlayerPosition, _player.transform.position) >= attackRange)
                {
                    UpdateTargetPositionAndPath();
                }
                else if (Vector3.Distance(_lastPlayerPosition, transform.position) <= attackRange && !isAttacking)
                {
                    Attack();
                }
            }
            else
            {
                HandlePatrolOrNewTarget();
            }
        }

        /// <summary>
        /// Sets the target position to the player's position and recalculates the path.
        /// </summary>
        private void StartFollowingPlayer()
        {
            _targetPosition.position = _player.transform.position;
            _lastPlayerPosition = _player.transform.position;
            RecalculatePath();
            _initialPathCalculated = true;
        }

        /// <summary>
        /// Updates the target position to the player's position and recalculates the path.
        /// </summary>
        private void UpdateTargetPositionAndPath()
        {
            canPathFindToTarget = true;
            _lastPlayerPosition = _player.transform.position;
            _targetPosition.position = _lastPlayerPosition;
            RecalculatePath();
        }

        /// <summary>
        /// When the enemy is not following the player, the enemy will either patrol or choose a new target.
        /// </summary>
        private void HandlePatrolOrNewTarget()
        {
            if (isFollowingPlayer)
            {
                if (!isTargetingPatrolPoints)
                {
                    ChooseNewTarget();
                }
                else
                {
                    PatrolPointFollower();
                }

                isFollowingPlayer = false;
            }
        }


        /// <summary>
        /// Depending on the type of enemy, the corresponding attack event is called so the relevant scripts and execute
        /// their attack behaviour.
        /// </summary>
        private void Attack()
        {
            if (enemyType == EnemyType.Melee)
            {
                Melee?.Invoke(gameObject);
            }
            else if (enemyType == EnemyType.Archer)
            {
                Ranged?.Invoke(gameObject);
            }
            else if (enemyType == EnemyType.Exploder)
            {
                Explode?.Invoke(gameObject);
            }
            else
            {
                Debug.LogWarning("Enemy type not set for the attack!");
            }
        }

        /// <summary>
        /// Calls the FindPath method while feeding in the start and target positions as the required Vectors
        /// </summary>
        private void RecalculatePath()
        {
            FindPath(_startPosition.position, _targetPosition.position);
        }

        /// <summary>
        /// Finds a path from the specified starting position to the target position using the A* pathfinding algorithm.
        /// </summary>
        /// <param name="startPos">The starting position in world coordinates.</param>
        /// <param name="targetPos">The target position in world coordinates.</param>
        /// <remarks>
        /// This method implements the A* algorithm to find the shortest path from the starting position to the target position.
        /// It maintains an open list of nodes to explore and a closed list of nodes that have already been explored.
        /// The algorithm iteratively selects the node with the least cost from the open list and explores its neighboring nodes
        /// until the target node is reached or there are no more nodes to explore.
        /// </remarks>
        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _gridReference.NodeFromWorldPoint(startPos);
            Node targetNode = _gridReference.NodeFromWorldPoint(targetPos);

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //If the f cost of that object is less than or equal to the f cost of the current node
                    if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost &&
                        openList[i].hCost < currentNode.hCost)
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
                    // Not a wall or already in the closed list
                    if (!neighbourNode.isWall || closedList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    //Get the F cost of that neighbor
                    var moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighbourNode);

                    //If the f cost is greater than the g cost or it is not in the open list
                    if (moveCost < neighbourNode.gCost || !openList.Contains(neighbourNode))
                    {
                        neighbourNode.gCost = moveCost;
                        neighbourNode.hCost = GetManhattanDistance(neighbourNode, targetNode);
                        neighbourNode.parentNode = currentNode;

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates the final path by traversing from the end node to the starting node.
        /// Each node is added to the final path list as the algorithm traverses through the parent nodes.
        /// The final path is then reversed to obtain the correct sequence.
        /// </summary>
        /// <param name="startingNode">The starting node of the path.</param>
        /// <param name="targetNode">The end node of the path.</param>
        private void GetFinalPath(Node startingNode, Node targetNode)
        {
            List<Node> finalPath = new List<Node>();
            Node currentNode = targetNode;

            _gridReference.SetEnemyFinalPath(gameObject, finalPath);

            // Work through each node going through the parents to the beginning of the path
            while (currentNode != startingNode)
            {
                finalPath.Add(currentNode);
                currentNode = currentNode.parentNode;
            }

            finalPath.Reverse();
            // Set the final path
            finalPathOfNodes = finalPath;
        }

        /// <summary>
        /// Retrieves the final path associated with the specified enemy GameObject.
        /// If a final path is found for the enemy, it returns the path; otherwise, it logs a warning message and returns null.
        /// </summary>
        /// <param name="enemy">The GameObject representing the enemy.</param>
        /// <returns>The final path of nodes for the specified enemy, or null if no path is found.</returns>
        public List<Node> GetFinalPath(GameObject enemy)
        {
            var enemyFinalPaths = _gridReference.EnemyFinalPaths;

            if (enemyFinalPaths.TryGetValue(enemy, out var path))
            {
                return path;
            }
            else
            {
                //Debug.LogWarning("No final path found for the specified enemy." + enemy);
                return null;
            }
        }

        /// <summary>
        /// Calculates the Manhattan distance between two nodes on the grid.
        /// </summary>
        /// <param name="nodeA">The first node.</param>
        /// <param name="nodeB">The second node.</param>
        /// <returns>The Manhattan distance between the two nodes.</returns>
        private int GetManhattanDistance(Node nodeA, Node nodeB)
        {
            var x = nodeA.gridX - nodeB.gridX;
            var y = nodeA.gridY - nodeB.gridY;

            // Use bitwise operations to compute absolute values
            x = (x ^ (x >> 31)) - (x >> 31);
            y = (y ^ (y >> 31)) - (y >> 31);

            return x + y;
        }

        /// <summary>
        /// Randomly chooses a new target position within the bounds of the grid and recalculates the path to the target.
        /// </summary>
        public void ChooseNewTarget()
        {
            var gridX = (_gridReference.gridWorldSize.x / _gridReference.nodeDiameter) / 2;
            var gridY = (_gridReference.gridWorldSize.y / _gridReference.nodeDiameter) / 2;

            _targetPosition.position = new Vector3(Random.Range(-gridX, gridX), 0,
                Random.Range(-gridY, gridY));
            RecalculatePath();
        }

        /// <summary>
        /// Follows a patrol path defined by a list of patrol points. 
        /// If no patrol points are set, a warning message is logged, and the method returns.
        /// The target position is set based on the current patrol index, and the path is recalculated.
        /// If moving forward, the current patrol index is incremented. If reaching the last patrol point,
        /// movement direction is reversed. On moving backward, the current patrol index is decremented.
        /// On reaching the first patrol point, movement direction is reversed.
        /// </summary>
        public void PatrolPointFollower()
        {
            if (patrolPointsList.Count == 0)
            {
                Debug.LogWarning("Patrol Points not set!");
                return;
            }

            // Set the target position based on the current patrol index
            _targetPosition.position = patrolPointsList[_currentPatrolPointIndex].transform.position;
            RecalculatePath();

            // Move to the next patrol or the previous one if reached the end
            if (_isMovingForward)
            {
                // Increment the current patrol index
                _currentPatrolPointIndex++;

                // If reached the last patrol, start moving backward
                if (_currentPatrolPointIndex >= patrolPointsList.Count)
                {
                    _currentPatrolPointIndex = patrolPointsList.Count - 2; // Set index to second-to-last patrol
                    _isMovingForward = false;
                }
            }
            else
            {
                // Decrement the current patrol index
                _currentPatrolPointIndex--;

                // If reached the first patrol, start moving forward
                if (_currentPatrolPointIndex < 0)
                {
                    _currentPatrolPointIndex = 1; // Set index to second patrol
                    _isMovingForward = true;
                }
            }
        }

        /// <summary>
        /// Visualises the aggro range of the enemy in the Scene view. Via a wire sphere.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Draw a wire sphere representing the sphere cast
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, aggroRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}


