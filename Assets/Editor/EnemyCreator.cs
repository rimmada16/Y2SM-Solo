using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCreator : EditorWindow
    {
        private GameObject _meleeEnemy, _archerEnemy, _exploderEnemy;
        //private SpawnManagerScriptableObject _enemyAttributes;

        private string[] _spawnOptions = { "Spawn in Front of Camera", "Spawn at Specific Location", "Spawn Anywhere", "Spawn on Selected GameObject" };
        private int _selectedSpawnOptionIndex; 
        private bool _spawnOnCamera, _spawnAtSpecificLocation, _spawnAnywhere, _spawnOnSelectedGameObject;
        private Vector3 _spawnLocation;
        private int _amountToSpawn = 1;
        private int _enemyHealth = 100;
        private Grid _gridReference;

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

        private EnemyType _selectedType = EnemyType.Melee;
        private MeleeWeaponType _selectedMeleeWeapon = MeleeWeaponType.Shortsword;
        private ArcherWeaponType _selectedArcherWeapon = ArcherWeaponType.Shortbow;
        private ExploderType _selectedExploderType = ExploderType.Small;

        private float _attackFrequency, _attackRange, _aggroRange, _attackDamage, _movementSpeed;

        /*private void OnEnable()
        {
            _enemyAttributes = CreateInstance<SpawnManagerScriptableObject>();
        }*/

        [MenuItem("Enemy Creator/Enemy Creator Window")]
        public static void ShowWindow()
        {
            EnemyCreator wnd = GetWindow<EnemyCreator>();
            wnd.titleContent = new GUIContent("Enemy Creator Window");
            
            // Ensure all content is visible
            wnd.minSize = new Vector2(400, 800);
        }
        
        private void OnGUI()
        {
            //
            // Selects the wanted enemy type
            //
        
            GUILayout.Label("Select Enemy Type", EditorStyles.boldLabel);
            GUILayout.Label("");

            if (GUILayout.Button("Melee"))
            {
                _selectedType = EnemyType.Melee;
                _aggroRange = 5;
            }

            if (GUILayout.Button("Archer"))
            {
                _selectedType = EnemyType.Archer;
                _aggroRange = 20;
            }

            if (GUILayout.Button("Exploder"))
            {
                _selectedType = EnemyType.Exploder;
                _aggroRange = 10;
            }

            //
            // Displays the wanted enemy type that has been selected
            //
        
            GUILayout.Label("Selected Enemy Type: " + _selectedType, EditorStyles.boldLabel);
            GUILayout.Label("");

            //
            // Do different things based on the selected enemy type
            // Allow selection of different weapons or types for the selected enemy type
            //
        
            switch (_selectedType)
            {
                // Melee allows selection of different melee weapons
                case EnemyType.Melee:
                    _meleeEnemy = (GameObject)EditorGUILayout.ObjectField("Melee Enemy Prefab:", _meleeEnemy, typeof(GameObject), true);

                    if (GUILayout.Button("Shortsword"))
                    {
                        _selectedMeleeWeapon = MeleeWeaponType.Shortsword;
                        _attackDamage = 10;
                        _attackFrequency = 2f;
                        _attackRange = 1.5f;
                        _aggroRange = 5;
                        _movementSpeed = 5;
                        _enemyHealth = 30;
                    }

                    if (GUILayout.Button("Longsword"))
                    {
                        // All need redoing
                        _selectedMeleeWeapon = MeleeWeaponType.Longsword;
                        _attackDamage = 15;
                        _attackFrequency = 3.5f;
                        _attackRange = 3;
                        _aggroRange = 5;
                        _movementSpeed = 3;
                        _enemyHealth = 60;
                    }

                    if (GUILayout.Button("Greatsword"))
                    {
                        // All need redoing
                        _selectedMeleeWeapon = MeleeWeaponType.Greatsword;
                        _attackDamage = 50;
                        _attackFrequency = 5f;
                        _attackRange = 3;
                        _aggroRange = 5;
                        _movementSpeed = 1.5f;
                        _enemyHealth = 100;
                    }

                    GUILayout.Label("Selected Melee weapon Type: " + _selectedMeleeWeapon, EditorStyles.boldLabel);
                    break;

                // Archer allows selection of different bows
                case EnemyType.Archer:
                    _archerEnemy = (GameObject)EditorGUILayout.ObjectField("Archer Enemy Prefab:", _archerEnemy, typeof(GameObject), true);

                    if (GUILayout.Button("Shortbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Shortbow;
                        _attackDamage = 10;
                        _attackFrequency = 1;
                        _attackRange = 9;
                        _movementSpeed = 5;
                        _enemyHealth = 20;
                    }

                    if (GUILayout.Button("Longbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Longbow;
                        _attackDamage = 15;
                        _attackFrequency = 0.75f;
                        _attackRange = 9;
                        _movementSpeed = 3;
                        _enemyHealth = 50;
                    }

                    if (GUILayout.Button("Greatbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Greatbow;
                        _attackDamage = 50;
                        _attackFrequency = 0.5f;
                        _attackRange = 9;
                        _movementSpeed = 1.5f;
                        _enemyHealth = 75;
                    }

                    GUILayout.Label("Selected Archer weapon Type: " + _selectedArcherWeapon, EditorStyles.boldLabel);
                    break;

                // Exploder allows selection of different exploder types
                case EnemyType.Exploder:
                    _exploderEnemy = (GameObject)EditorGUILayout.ObjectField("Exploder Enemy Prefab:", _exploderEnemy, typeof(GameObject), true);

                    if (GUILayout.Button("Small"))
                    {
                        _selectedExploderType = ExploderType.Small;
                        _attackDamage = 25;
                        _attackFrequency = 1;
                        _attackRange = 1;
                        _movementSpeed = 3;
                        _enemyHealth = 50;
                    }

                    if (GUILayout.Button("Medium"))
                    {
                        _selectedExploderType = ExploderType.Medium;
                        _attackDamage = 50;
                        _attackFrequency = 1;
                        _attackRange = 2;
                        _movementSpeed = 2;
                        _enemyHealth = 100;
                    }

                    if (GUILayout.Button("Nuke"))
                    {
                        _selectedExploderType = ExploderType.Nuke;
                        _attackDamage = 200;
                        _attackFrequency = 0.5f;
                        _attackRange = 2;
                        _movementSpeed = 1.5f;
                        _enemyHealth = 200;
                    }

                    GUILayout.Label("Selected Exploder Type: " + _selectedExploderType, EditorStyles.boldLabel);
                    break;
            }
        
            GUILayout.Label("");
        
            // Allows user to enter the attack speed attribute for the enemy
            GUILayout.Label("Enter Attack Frequency", EditorStyles.boldLabel);
            _attackFrequency = EditorGUILayout.FloatField("Attack Frequency:", _attackFrequency);

            GUILayout.Label("Enter Aggro Range", EditorStyles.boldLabel);
            _aggroRange = EditorGUILayout.FloatField("Aggro Range:", _aggroRange);
            
            // Allows user to enter the aggro range attribute for the enemy
            GUILayout.Label("Enter Attack Range", EditorStyles.boldLabel);
            _attackRange = EditorGUILayout.FloatField("Attack Range:", _attackRange);

            // Allows user to enter the attack damage attribute for the enemy
            GUILayout.Label("Enter Attack Damage", EditorStyles.boldLabel);
            _attackDamage = EditorGUILayout.FloatField("Attack Damage:", _attackDamage);

            // Allows user to enter the movement speed attribute for the enemy
            GUILayout.Label("Enter Movement Speed", EditorStyles.boldLabel);
            _movementSpeed = EditorGUILayout.FloatField("Movement Speed:", _movementSpeed);
            
            // Allows the user to enter the HP of the enemy
            GUILayout.Label("Enter the HP of the Unit", EditorStyles.boldLabel);
            _enemyHealth = EditorGUILayout.IntField("HP:", _enemyHealth);

        
            //GUILayout.Space(10);
            GUILayout.Label("");
        
            // Allows the user to select from multiple different spawn locations
            GUILayout.Label("Select Spawn Option", EditorStyles.boldLabel);
            _selectedSpawnOptionIndex = EditorGUILayout.Popup("Spawn Option:", _selectedSpawnOptionIndex, _spawnOptions);
        
            switch (_selectedSpawnOptionIndex)
            {
                case 0:
                    var cameraTransform = SceneView.lastActiveSceneView.camera.transform;
                    _spawnLocation = cameraTransform.position + cameraTransform.forward * 2f;
                    // Handle spawning in front of camera
                    break;

                case 1:
                    GUILayout.Label("");
                    GUILayout.Label("Enter Spawn Location", EditorStyles.boldLabel);
                    _spawnLocation = new Vector3(21, 0, 21);
                    _spawnLocation = EditorGUILayout.Vector3Field("Spawn Location:", _spawnLocation);
                    // Handle spawning at specific position
                    break;

                case 2:
                    _spawnLocation = new Vector3(Random.Range(0, 25), 0, Random.Range(0, 25));
                    
                    // Handle spawning anywhere
                    break;
                
                case 3:
                    if (Selection.activeGameObject != null)
                    {
                        _spawnOnSelectedGameObject = true;
                        _spawnLocation = Selection.activeGameObject.transform.position;
                    }
                    else
                    {
                        _spawnOnSelectedGameObject = false;
                        GUILayout.Label("");
                        GUILayout.Label("WARNING: No GameObject selected to spawn on.");
                        GUILayout.Label("Please select a GameObject to spawn on.");
                    }
                    break;
            }
            
            GUILayout.Label("");
        
            // Allows the user to enter the amount of enemies to spawn
            GUILayout.Label("Enter amount to Spawn", EditorStyles.boldLabel);
            _amountToSpawn = EditorGUILayout.IntSlider("Amount to Spawn:", _amountToSpawn, 1, 10);
        
            GUILayout.Label("");
            
            // Will instantiate the enemy with the selected attributes
            if (GUILayout.Button("Create Enemy"))
            {
                GameObject objectToInstantiate;

                if (_selectedSpawnOptionIndex == 3 && !_spawnOnSelectedGameObject)
                {
                    return;
                }
                
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
                    
                    
                    
                    enemyHealth.health = _enemyHealth;
            
                    // Assign the attributes to the enemy
                    if (pathfinding != null && enemyMovement != null)
                    {
                        //enemyScript.enemyAttributes = _enemyAttributes;
                        
                        // Probably shouldn't be in pathfinding? but then again state change based on pathfinding
                        pathfinding.attackFrequency = _attackFrequency;
                        pathfinding.aggroRange = _aggroRange;
                        pathfinding.attackRange = _attackRange;
                        enemyMovement.movementSpeed = _movementSpeed;

                        if (_selectedType == EnemyType.Melee)
                        {
                            // Find which weapon is selected, then get the corresponding child object and set it to active
                            if (_selectedMeleeWeapon == MeleeWeaponType.Longsword)
                            {
                                Transform meshTransform = newEnemy.transform.Find("Mesh");
                                if (meshTransform != null)
                                {
                                    Debug.Log("Mesh found");
                                    Transform weaponTransform = meshTransform.Find("Longsword");
                                    if (weaponTransform != null)
                                    {
                                        Debug.Log("Longsword found");
                                        // Setting the child GameObject active
                                        weaponTransform.gameObject.SetActive(true);
                                        DealDamage dealDamage = weaponTransform.GetComponent<DealDamage>();
                                        dealDamage.damage = (int)_attackDamage;
                                    }
                                }
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Greatsword)
                            {
                                Transform meshTransform = newEnemy.transform.Find("Mesh");
                                if (meshTransform != null)
                                {
                                    Debug.Log("Mesh found");
                                    Transform weaponTransform = meshTransform.Find("Greatsword");
                                    if (weaponTransform != null)
                                    {
                                        Debug.Log("Greatsword found");
                                        // Setting the child GameObject active
                                        weaponTransform.gameObject.SetActive(true);
                                        DealDamage dealDamage = weaponTransform.GetComponent<DealDamage>();
                                        dealDamage.damage = (int)_attackDamage;
                                    }
                                }
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Shortsword)
                            {
                                Transform meshTransform = newEnemy.transform.Find("Mesh");
                                if (meshTransform != null)
                                {
                                    Debug.Log("Mesh found");
                                    Transform weaponTransform = meshTransform.Find("Shortsword");
                                    if (weaponTransform != null)
                                    {
                                        Debug.Log("Shortsword found");
                                        // Setting the child GameObject active
                                        weaponTransform.gameObject.SetActive(true);
                                        DealDamage dealDamage = weaponTransform.GetComponent<DealDamage>();
                                        dealDamage.damage = (int)_attackDamage;
                                    }
                                }
                            } 
                        }

                        if (_selectedType == EnemyType.Archer)
                        {
                            // Find which weapon is selected, then get the corresponding child object and set it to active
                            if (_selectedArcherWeapon == ArcherWeaponType.Longbow)
                            {
                                Transform childTransform = newEnemy.transform.GetChild(0);
        
                                // Checking if a child exists
                                if (childTransform != null)
                                {
                                    // Setting the child GameObject active
                                    childTransform.gameObject.SetActive(true);
                                }
                            }
                            else if (_selectedArcherWeapon == ArcherWeaponType.Greatbow)
                            {
                                Transform childTransform = newEnemy.transform.GetChild(1);
        
                                // Checking if a child exists
                                if (childTransform != null)
                                {
                                    // Setting the child GameObject active
                                    childTransform.gameObject.SetActive(true);
                                }
                            }
                            else if (_selectedArcherWeapon == ArcherWeaponType.Shortbow)
                            {
                                Transform childTransform = newEnemy.transform.GetChild(2);
        
                                // Checking if a child exists
                                if (childTransform != null)
                                {
                                    // Setting the child GameObject active
                                    childTransform.gameObject.SetActive(true);
                                }
                            }
                        }

                        if (_selectedType == EnemyType.Exploder)
                        {
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
        }
    }

            /*if (GUILayout.Button("Create Attributes for Enemy"))
        {
            SpawnManagerScriptableObject newEnemyAttributes = CreateInstance<SpawnManagerScriptableObject>();
            newEnemyAttributes.attackSpeed = _attackSpeed;
            newEnemyAttributes.attackRange = _attackRange;
            newEnemyAttributes.attackDamage = _attackDamage;
            newEnemyAttributes.movementSpeed = _movementSpeed;

            string path = EditorUtility.SaveFilePanelInProject("Save Enemy Attribute", "NewEnemyAttribute", "asset", "Save enemy attribute");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newEnemyAttributes, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = newEnemyAttributes;
            }
        }*/
    
    
