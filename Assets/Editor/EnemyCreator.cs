// Purpose: To create a custom editor window that allows the user to create enemies with different attributes and weapons.

using System.Collections.Generic;
using AStarPathfinding;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor
{
    /// <summary>
    /// An editor window that allows the user to create enemies within the scene from 3 selected types being a Melee Unit,
    /// a Archer Unit and an Exploder Unit. Depending on the Unit type selected the user can change the subtype of the Unit
    /// and edit different attributes for the unit. i.e. if the Archer Unit is selected then the user can change the type
    /// of arrow they shoot. The user can also select whether the enemy will roam freely within the bounds of the map grid
    /// or follow set patrol points defined by the user. The user can also select where they would like to spawn the enemy
    /// with multiple different spawning options. Finally the user can select how many units they would like to spawn
    /// within the appropriate predefined range.
    /// </summary>
    public class EnemyCreator : EditorWindow
    {
        // For the input of the prefab for the enemy
        private GameObject _meleeEnemy, _archerEnemy, _exploderEnemy;

        // For the selection of camera spawn options from drop down
        private readonly string[] _spawnOptions = { "Spawn in Front of Camera", "Spawn at Specific Location", "Spawn Anywhere", "Spawn on Selected GameObject" };
        
        // For the selection of an agent type from drop down
        private readonly string[] _agentTypes = { "Roam Bounds", "Follow Patrol Points" };
        
        // For the selection of an arrow type from drop down
        private readonly string[] _arrowTypes = { "Standard Arrow", "Broadhead Arrow", "Greatbolts" };
        
        // For selection of enemy type
        private readonly string[] _enemyTypes = { "Melee", "Archer", "Exploder" };
        
        // For selection of enemy sub type from drop down
        private readonly string[] _meleeEnemyTypes = { "Shortsword", "Longsword", "Greatsword" };
        private readonly string[] _archerEnemyTypes = { "Shortbow", "Longbow", "Greatbow" };
        private readonly string[] _exploderEnemyTypes = { "Small", "Medium", "Nuke" };
        
        // Camera index - Roam or Patrol index - Arrow type index
        private int _selectedSpawnOptionIndex, _selectedAgentTypeIndex, _selectedArrowTypeIndex;

        // For setting the _selectedType
        private int _selectedEnemyTypeIndex;
        
        // For applying default values
        private int _selectedMeleeEnemyIndex, _selectedArcherEnemyIndex, _selectedExploderEnemyIndex;
        
        private bool _spawnOnSelectedGameObject;
        
        // For the enemy spawn location
        private Vector3 _spawnLocation;
        
        private int _amountToSpawn = 1;
        private int _enemyHealth = 100;

        private enum EnemyType
        {
            Melee, Archer, Exploder
        }

        private enum MeleeWeaponType
        {
            Shortsword, Longsword, Greatsword
        }

        private enum ArcherWeaponType
        {
            Shortbow, Longbow, Greatbow
        }

        private enum ExploderType
        {
            Small, Medium, Nuke
        }

        private enum SelectionOption
        {
            RoamBounds, FollowPatrolPoints
        }

        // Enum stuff
        private SelectionOption _selectedOption = SelectionOption.RoamBounds;
        private EnemyType _selectedType = EnemyType.Melee;
        private MeleeWeaponType _selectedMeleeWeapon = MeleeWeaponType.Shortsword;
        private ArcherWeaponType _selectedArcherWeapon = ArcherWeaponType.Shortbow;
        private ExploderType _selectedExploderType = ExploderType.Small;

        // Enemy attributes
        private float _timeBetweenAttacks, _attackRange, _explosionRadius, _aggroRange, _attackRangeUpperBound, _movementSpeed;
        private int _attackDamage;
        
        // Patrol points
        private readonly List<Transform> _patrolPoints = new();
        private Transform _newPatrolPoint;
        
        // Scroll view
        private Vector2 _scrollPosition = Vector2.zero;


        private bool _useDefaultValues = true;
        
        
        /// <summary>
        /// MenuItem: Editor window can be opened via the Unity menu
        /// ShowWindow: Creates the window in the editor so it is visible for the user
        /// </summary>
        [MenuItem("Enemy Creator/Enemy Creator Window")]
        public static void ShowWindow()
        {
            EnemyCreator wnd = GetWindow<EnemyCreator>();
            wnd.titleContent = new GUIContent("Enemy Creator Window");
            
            // Ensure all content is visible
            wnd.minSize = new Vector2(400, 400);
        }

        /// <summary>
        /// Rendering and Handling of GUI Events
        /// </summary>
        private void OnGUI()
        {
            // Scroll bar
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            //Set Initial Values
            _selectedType = EnemyType.Melee;
            _selectedMeleeWeapon = MeleeWeaponType.Shortsword;
            
            EditorGUILayout.Space();
            // Gives the user the option to select the enemy type they wish to spawn
            EditorGUILayout.LabelField(new GUIContent("Spawn Option:", "Select what type of enemy you wish to spawn"), EditorStyles.boldLabel);
            _selectedEnemyTypeIndex = EditorGUILayout.Popup(_selectedEnemyTypeIndex, _enemyTypes);
            if (GUI.changed)
            {
                _useDefaultValues = true;
            }

            switch (_selectedEnemyTypeIndex)
            {
                case 0:
                    _selectedType = EnemyType.Melee;
                    break;
                case 1:
                    _selectedType = EnemyType.Archer;
                    break;
                case 2:
                    _selectedType = EnemyType.Exploder;
                    break;
            }
            
            EditorGUILayout.Space();
            
            // Do different things based on the selected enemy type
            // Allow selection of different weapons or types for the selected enemy type
            switch (_selectedType)
            {
                // Melee allows selection of different melee weapons
                case EnemyType.Melee:
                    _meleeEnemy = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Melee Enemy Prefab:", "Please enter the corresponding prefab to the selected enemy type"), _meleeEnemy, typeof(GameObject), true);

                    _selectedMeleeEnemyIndex = EditorGUILayout.Popup(new GUIContent("Melee Enemy Type:", "Select a type of Melee Unit"), _selectedMeleeEnemyIndex, _meleeEnemyTypes);
                    if (GUI.changed)
                    {
                        _useDefaultValues = true;
                    }

                    switch (_selectedMeleeEnemyIndex)
                    {
                        case 0:
                            if (_useDefaultValues)
                            {
                                _selectedMeleeWeapon = MeleeWeaponType.Shortsword;
                                _attackDamage = 10;
                                _timeBetweenAttacks = 2f;
                                _attackRange = _attackRangeUpperBound = 1.5f;
                                _aggroRange = 5;
                                _movementSpeed = 5;
                                _enemyHealth = 30;
                                _useDefaultValues = false;
                            }
                            break;
                        
                        case 1:
                            if (_useDefaultValues)
                            {
                                _selectedMeleeWeapon = MeleeWeaponType.Longsword;
                                _attackDamage = 15;
                                _timeBetweenAttacks = 3.5f;
                                _attackRange = _attackRangeUpperBound = 3;
                                _aggroRange = 5;
                                _movementSpeed = 3;
                                _enemyHealth = 60;
                                _useDefaultValues = false;
                            }
                            break;
                        
                        case 2:
                            if (_useDefaultValues)
                            {
                                _selectedMeleeWeapon = MeleeWeaponType.Greatsword;
                                _attackDamage = 50;
                                _timeBetweenAttacks = 5f;
                                _attackRange = _attackRangeUpperBound = 3;
                                _aggroRange = 5;
                                _movementSpeed = 1.5f;
                                _enemyHealth = 100;
                                _useDefaultValues = false;
                            }
                            break;
                            
                    }
                    break;

                // Archer allows selection of different bows
                case EnemyType.Archer:
                    _archerEnemy = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Archer Enemy Prefab:", "Please enter the corresponding prefab to the selected enemy type"), _archerEnemy, typeof(GameObject), true);

                    _selectedArcherEnemyIndex = EditorGUILayout.Popup(new GUIContent("Archer Enemy Type:", "Select a type of Archer Unit"), _selectedArcherEnemyIndex, _archerEnemyTypes);
                    if (GUI.changed)
                    {
                        _useDefaultValues = true;
                    }

                    switch (_selectedArcherEnemyIndex)
                    {
                        case 0:
                            if (_useDefaultValues)
                            {
                                _selectedArcherWeapon = ArcherWeaponType.Shortbow;
                                _attackDamage = 10;
                                _timeBetweenAttacks = 2;
                                _attackRange = _attackRangeUpperBound = 9;
                                _aggroRange = 20;
                                _movementSpeed = 5;
                                _enemyHealth = 20;
                                _useDefaultValues = false;
                            }
                            break;
                        case 1:
                            if (_useDefaultValues)
                            {
                                _selectedArcherWeapon = ArcherWeaponType.Longbow;
                                _attackDamage = 15;
                                _timeBetweenAttacks = 3.5f;
                                _attackRange = _attackRangeUpperBound = 9;
                                _aggroRange = 20;
                                _movementSpeed = 3;
                                _enemyHealth = 50;
                                _useDefaultValues = false;
                            }
                            break;
                        case 2:
                            if (_useDefaultValues)
                            {
                                _selectedArcherWeapon = ArcherWeaponType.Greatbow;
                                _attackDamage = 50;
                                _timeBetweenAttacks = 5f;
                                _attackRange = _attackRangeUpperBound = 9;
                                _aggroRange = 20;
                                _movementSpeed = 1.5f;
                                _enemyHealth = 75;
                                _useDefaultValues = false;
                            }
                            break;
                    }
                    break;

                // Exploder allows selection of different exploder types
                case EnemyType.Exploder:
                    _exploderEnemy = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Exploder Enemy Prefab:", "Please enter the corresponding prefab to the selected enemy type"), _exploderEnemy, typeof(GameObject), true);

                    _selectedExploderEnemyIndex = EditorGUILayout.Popup(new GUIContent("Exploder Enemy Type:", "Select a type of Exploder Unit"), _selectedExploderEnemyIndex, _exploderEnemyTypes);
                    if (GUI.changed)
                    {
                        _useDefaultValues = true;
                    }

                    switch (_selectedExploderEnemyIndex)
                    {
                        case 0:
                            if (_useDefaultValues)
                            {
                                _selectedExploderType = ExploderType.Small;
                                _attackDamage = 25;
                                _timeBetweenAttacks = 1;
                                _attackRange = _attackRangeUpperBound = 1.5f;
                                _aggroRange = 10;
                                _explosionRadius = 3f;
                                _movementSpeed = 3;
                                _enemyHealth = 50;
                                _useDefaultValues = false;
                            }
                            break;
                        case 1:
                            if (_useDefaultValues)
                            {
                                _selectedExploderType = ExploderType.Medium;
                                _attackDamage = 50;
                                _timeBetweenAttacks = 1;
                                _attackRange = _attackRangeUpperBound = 2;
                                _aggroRange = 10;
                                _explosionRadius = 4.5f;
                                _movementSpeed = 2;
                                _enemyHealth = 100;
                                _useDefaultValues = false;
                            }
                            break;
                        case 2:
                            if (_useDefaultValues)
                            {
                                _selectedExploderType = ExploderType.Nuke;
                                _attackDamage = 200;
                                _timeBetweenAttacks = 1f;
                                _attackRange = _attackRangeUpperBound = 3.5f;
                                _aggroRange = 10;
                                _explosionRadius = 6.5f;
                                _movementSpeed = 1.5f;
                                _enemyHealth = 200;
                                _useDefaultValues = false;
                            }
                            break;
                    }
                    break;
            }
        
            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent("Enemy Attributes Section", "Set the attributes for the enemy"), EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Allows user to enter the attack speed attribute for the enemy
            _timeBetweenAttacks = EditorGUILayout.Slider(new GUIContent("Time between Attacks:", "Enter the time between each attack for the enemy, their attack cooldown"), _timeBetweenAttacks, 1, 5);
            
            // Allows the user to enter the aggression range attribute for the enemy
            _aggroRange = EditorGUILayout.Slider(new GUIContent("Aggro Range:", "Enter the range in which the enemy will start tracking the player"), _aggroRange, 4.5f, 20);
            
            // Allows user to enter the aggro range attribute for the enemy
            _attackRange = EditorGUILayout.Slider(new GUIContent("Attack Range:", "Enter the range in which the enemy must be from the player before it will start attacking"), _attackRange, 1.5f, _attackRangeUpperBound);
            
            
            // Ensure that the aggro range is always greater than the attack range by 3 units
            if (_aggroRange - _attackRange <= 3)
            {
                _aggroRange = _attackRange + 3;
            }
            
            if (_selectedType != EnemyType.Archer)
            {
                // Allows user to enter the attack damage attribute for the enemy
                _attackDamage = EditorGUILayout.IntSlider(new GUIContent("Attack Damage:", "Enter the attack damage of the enemy"), _attackDamage, 1, 200);
            }

            
            if (_selectedType == EnemyType.Archer)
            {
                // Allows the user to select the arrow type for the archer
                _selectedArrowTypeIndex = EditorGUILayout.Popup(new GUIContent("Arrow Type:", "Select the arrow type that the archer will shoot"), _selectedArrowTypeIndex, _arrowTypes);
            }

            if (_selectedType == EnemyType.Exploder)
            {
                // Allows the user to enter the explosion radius attribute for the enemy
                _explosionRadius = EditorGUILayout.Slider(new GUIContent("Explosion Radius:", "Enter the explosion radius of the Exploder Unit"), _explosionRadius, 2.5f, 10);
            }
            
            // Allows user to enter the movement speed attribute for the enemy
            _movementSpeed = EditorGUILayout.Slider(new GUIContent("Movement Speed:", "Enter the speed at which the enemy can move"), _movementSpeed, 1.5f, 10);

            // Allows the user to enter the HP of the enemy
            _enemyHealth = EditorGUILayout.IntSlider(new GUIContent("HP:", "Enter the health points for the Enemy"), _enemyHealth, 1, 200);

            EditorGUILayout.Space();
            
            GUILayout.Label("Select Agent Type", EditorStyles.boldLabel);
            // Allows the user to select the agent type which will make it so the agent is either roaming the bounds of the grid or following set patrol points
            _selectedAgentTypeIndex = EditorGUILayout.Popup(new GUIContent("Agent Type:", "Select whether the enemy will roam the bounds of the grid or follow patrol points"), _selectedAgentTypeIndex, _agentTypes);

            switch (_selectedAgentTypeIndex)
            {
                case 0:
                    _selectedOption = SelectionOption.RoamBounds;
                    break;
                case 1:
                    _selectedOption = SelectionOption.FollowPatrolPoints;
                    break;
            }
            
            if (_selectedOption == SelectionOption.FollowPatrolPoints)
            {
                GUILayout.BeginHorizontal();
                // Option to add Patrol Points to the list
                _newPatrolPoint = (Transform)EditorGUILayout.ObjectField(_newPatrolPoint, typeof(Transform), true);
                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {
                    if (_newPatrolPoint != null && !_patrolPoints.Contains(_newPatrolPoint))
                    {
                        _patrolPoints.Add(_newPatrolPoint);
                        _newPatrolPoint = null;
                    }
                    
                }
                GUILayout.EndHorizontal();
                // Lists the Patrol Points on the editor window with an option to remove the Patrol Points in the list
                GUILayout.Label(new GUIContent("Current Patrol Points", "The current list of patrol points that the enemy will follow"), EditorStyles.boldLabel);
                for (int i = 0; i < _patrolPoints.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(_patrolPoints[i], typeof(Transform), true);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        _patrolPoints.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                
                // Display error message and prevent enemy spawning if patrol points are being used and there is
                // less than 2 in the list
                if (_selectedOption == SelectionOption.FollowPatrolPoints && _patrolPoints.Count < 2)
                {
                    EditorGUILayout.HelpBox("At least 2 patrol points are required for enemies to follow patrol points.", MessageType.Error);
                    GUILayout.EndScrollView();
                    return;
                }
            }
            
            EditorGUILayout.Space();
        
            // Allows the user to select from multiple different spawn locations
            GUILayout.Label(new GUIContent("Select Spawn Option", "Select the type of spawn option to be used for the enemy"), EditorStyles.boldLabel);
            _selectedSpawnOptionIndex = EditorGUILayout.Popup("Spawn Option:", _selectedSpawnOptionIndex, _spawnOptions);
        
            switch (_selectedSpawnOptionIndex)
            {
                case 0:
                    // Handle spawning in front of camera
                    var cameraTransform = SceneView.lastActiveSceneView.camera.transform;
                    _spawnLocation = cameraTransform.position + cameraTransform.forward * 2f;
                    break;

                case 1:
                    // Handle spawning at specific position
                    EditorGUILayout.Space();
                    GUILayout.Label(new GUIContent("Enter Spawn Location", "Enter the X, Y, and Z Coordinates to spawn the enemy at"), EditorStyles.boldLabel);
                    _spawnLocation = new Vector3(21, 0, 21);
                    _spawnLocation = EditorGUILayout.Vector3Field("Spawn Location:", _spawnLocation);
                    
                    break;

                case 2:
                    // Handle spawning anywhere
                    _spawnLocation = new Vector3(Random.Range(0, 25), 0, Random.Range(0, 25));
                    break;
                
                case 3:
                    // Handle spawning on the selected GameObject in the scene view
                    if (Selection.activeGameObject != null)
                    {
                        _spawnOnSelectedGameObject = true;
                        _spawnLocation = Selection.activeGameObject.transform.position;
                    }
                    else
                    {
                        _spawnOnSelectedGameObject = false;
                        EditorGUILayout.Space();
                        // Display error message if there is not a GameObject selected
                        EditorGUILayout.HelpBox("A GameObject must be selected for this feature.", MessageType.Error);
                        GUILayout.EndScrollView();
                        return;
                    }
                    break;
            }
            
            EditorGUILayout.Space();
        
            // Allows the user to enter the amount of enemies to spawn
            GUILayout.Label("Enter amount to Spawn", EditorStyles.boldLabel);
            _amountToSpawn = EditorGUILayout.IntSlider("Amount to Spawn:", _amountToSpawn, 1, 10);
        
            EditorGUILayout.Space();
            
            // Will instantiate the enemy with the selected attributes
            if (GUILayout.Button("Create Enemy"))
            {
                GameObject objectToInstantiate;

                if (_selectedSpawnOptionIndex == 3 && !_spawnOnSelectedGameObject)
                {
                    return;
                }
                
                
                
                // Sets the var to the prefab
                if (_selectedType == EnemyType.Melee && _meleeEnemy != null)
                {
                    objectToInstantiate = _meleeEnemy;
                }
                else if (_selectedType == EnemyType.Archer && _archerEnemy != null)
                {
                    objectToInstantiate = _archerEnemy;
                }
                else if (_selectedType == EnemyType.Exploder && _exploderEnemy != null)
                {
                    objectToInstantiate = _exploderEnemy;
                }
                else
                {
                    Debug.LogWarning("Enemy type not recognized.");
                    return;
                }

                // For loop to spawn the required amount of enemies
                for (int i = 0; i < _amountToSpawn; i++)
                {
                    GameObject newEnemy = Instantiate(objectToInstantiate,_spawnLocation, Quaternion.identity);
                    
                    // Get the relevant scripts off the enemy
                    Pathfinding pathfinding = newEnemy.GetComponent<Pathfinding>();
                    EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
                    Health enemyHealth = newEnemy.GetComponent<Health>();

                    if (_selectedType == EnemyType.Exploder)
                    {
                        ExploderUnit exploderUnit = newEnemy.GetComponent<ExploderUnit>();
                        if (exploderUnit != null)
                        {
                            exploderUnit.explosionRadius = _explosionRadius;
                            exploderUnit.baseDamage = Mathf.RoundToInt(_attackDamage);
                        }
                    }
                    
                    // Set the enemies health
                    enemyHealth.health = _enemyHealth;
            
                    // Assign the attributes to the enemy
                    if (pathfinding != null && enemyMovement != null)
                    {
                        pathfinding.aggroRange = _aggroRange;
                        pathfinding.attackRange = _attackRange;
                        pathfinding.selectionOption = (Pathfinding.SelectionOption)_selectedOption;
                        enemyMovement.movementSpeed = _movementSpeed;

                        // Set the Patrol Points in the enemy
                        if (_selectedOption == SelectionOption.FollowPatrolPoints)
                        {
                            pathfinding.patrolPointsList = _patrolPoints;
                        }

                        if (_selectedType == EnemyType.Melee)
                        {
                            pathfinding.enemyType = Pathfinding.EnemyType.Melee;
                            // Find which weapon is selected, then get the corresponding child object and set it to active
                            if (_selectedMeleeWeapon == MeleeWeaponType.Longsword)
                            {
                                EnableCorrespondingWeapon(_selectedMeleeWeapon.ToString(), newEnemy, EnemyType.Melee);
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Greatsword)
                            {
                                EnableCorrespondingWeapon(_selectedMeleeWeapon.ToString(), newEnemy, EnemyType.Melee);
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Shortsword)
                            {
                                EnableCorrespondingWeapon(_selectedMeleeWeapon.ToString(), newEnemy, EnemyType.Melee);
                            } 
                        }

                        if (_selectedType == EnemyType.Archer)
                        {
                            pathfinding.enemyType = Pathfinding.EnemyType.Archer;
                            // Find which weapon is selected, then get the corresponding child object and set it to active
                            if (_selectedArcherWeapon == ArcherWeaponType.Longbow)
                            {
                                EnableCorrespondingWeapon(_selectedArcherWeapon.ToString(), newEnemy, EnemyType.Archer);
                            }
                            else if (_selectedArcherWeapon == ArcherWeaponType.Greatbow)
                            {
                                EnableCorrespondingWeapon(_selectedArcherWeapon.ToString(), newEnemy, EnemyType.Archer);
                            }
                            else if (_selectedArcherWeapon == ArcherWeaponType.Shortbow)
                            {
                                EnableCorrespondingWeapon(_selectedArcherWeapon.ToString(), newEnemy, EnemyType.Archer);
                            }
                        }

                        if (_selectedType == EnemyType.Exploder)
                        {
                            pathfinding.enemyType = Pathfinding.EnemyType.Exploder;
                            // Apply appropriate scale depending on the exploder type
                            if (_selectedExploderType == ExploderType.Small)
                            {
                                newEnemy.transform.localScale = new Vector3(1, .5f, 1);
                            }
                            else if (_selectedExploderType == ExploderType.Medium)
                            {
                                newEnemy.transform.localScale = new Vector3(1, 1, 1);
                            }
                            else if (_selectedExploderType == ExploderType.Nuke)
                            {
                                newEnemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                            }
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        
        /// <summary>
        /// This method finds a transform called "Mesh" on the param newEnemy, it will then find Mesh transform that is
        /// the same as the string param weaponName on the Mesh transform, once that is found it will set it to active.
        /// If the enemy type is Melee type then it will find the DealDamage script on the weaponName mesh and apply
        /// the _attackDamage var to the DealDamage scripts damage var.
        /// </summary>
        /// <param name="weaponName"></param>
        /// <param name="newEnemy"></param>
        /// <param name="enemyType"></param>
        private void EnableCorrespondingWeapon(string weaponName, GameObject newEnemy, EnemyType enemyType)
        {
            // Get the mesh
            Transform meshTransform = newEnemy.transform.Find("Mesh");
            if (meshTransform != null)
            {
                Debug.Log("Mesh found");
                // Get the appropriate weapon off the mesh
                Transform weaponTransform = meshTransform.Find(weaponName);
                if (weaponTransform != null)
                {
                    Debug.Log(weaponName + " found");
                    // Enable the appropriate weapon
                    weaponTransform.gameObject.SetActive(true);

                    switch (enemyType)
                    {
                        case EnemyType.Melee:
                        {
                            DealDamage dealDamage = weaponTransform.GetComponent<DealDamage>();
                            if (dealDamage != null)
                            {
                                dealDamage.damage = Mathf.RoundToInt(_attackDamage);
                            }
                            MeleeUnit meleeUnit = newEnemy.GetComponent<MeleeUnit>();
                            if (meleeUnit != null)
                            {
                                meleeUnit.timeBetweenAttacks = _timeBetweenAttacks;
                            }
                            break;
                        }
                        case EnemyType.Archer:
                            ArcherUnit archerUnit = newEnemy.GetComponent<ArcherUnit>();
                            if (archerUnit != null)
                            {
                                archerUnit.timeBetweenAttacks = _timeBetweenAttacks;
                                archerUnit.arrowToShoot = _selectedArrowTypeIndex;
                            }
                            break;
                    }
                }
            }
        }
    }
}


    
    
