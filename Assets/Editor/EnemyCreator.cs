using UnityEditor;
using UnityEngine;

    public class EnemyCreator : EditorWindow
    {
        private GameObject _meleeEnemy, _archerEnemy, _exploderEnemy;
        //private SpawnManagerScriptableObject _enemyAttributes;

        private string[] _spawnOptions = { "Spawn in Front of Camera", "Spawn at Specific Location", "Spawn Anywhere", "Spawn on Selected GameObject" };
        private int _selectedSpawnOptionIndex; 
        private bool _spawnOnCamera, _spawnAtSpecificLocation, _spawnAnywhere, _spawnOnSelectedGameObject;
        private Vector3 _spawnLocation;
        private int _amountToSpawn = 1;
        private int _enemyHP = 100;

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

        private float _attackSpeed, _attackRange, _aggroRange, _attackDamage, _movementSpeed;

        /*private void OnEnable()
        {
            _enemyAttributes = CreateInstance<SpawnManagerScriptableObject>();
        }*/

        [MenuItem("Enemy Creator/Enemy Creator Window")]
        public static void ShowExample()
        {
            EnemyCreator wnd = GetWindow<EnemyCreator>();
            wnd.titleContent = new GUIContent("Enemy Creator Window");
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
            }

            if (GUILayout.Button("Archer"))
            {
                _selectedType = EnemyType.Archer;
            }

            if (GUILayout.Button("Exploder"))
            {
                _selectedType = EnemyType.Exploder;
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
                    }

                    if (GUILayout.Button("Longsword"))
                    {
                        _selectedMeleeWeapon = MeleeWeaponType.Longsword;
                    }

                    if (GUILayout.Button("Greatsword"))
                    {
                        _selectedMeleeWeapon = MeleeWeaponType.Greatsword;
                    }

                    GUILayout.Label("Selected Melee weapon Type: " + _selectedMeleeWeapon, EditorStyles.boldLabel);
                    break;

                // Archer allows selection of different bows
                case EnemyType.Archer:
                    _archerEnemy = (GameObject)EditorGUILayout.ObjectField("Archer Enemy Prefab:", _archerEnemy, typeof(GameObject), true);

                    if (GUILayout.Button("Shortbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Shortbow;
                    }

                    if (GUILayout.Button("Longbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Longbow;
                    }

                    if (GUILayout.Button("Greatbow"))
                    {
                        _selectedArcherWeapon = ArcherWeaponType.Greatbow;
                    }

                    GUILayout.Label("Selected Archer weapon Type: " + _selectedArcherWeapon, EditorStyles.boldLabel);
                    break;

                // Exploder allows selection of different exploder types
                case EnemyType.Exploder:
                    _exploderEnemy = (GameObject)EditorGUILayout.ObjectField("Exploder Enemy Prefab:", _exploderEnemy, typeof(GameObject), true);

                    if (GUILayout.Button("Small"))
                    {
                        _selectedExploderType = ExploderType.Small;
                    }

                    if (GUILayout.Button("Medium"))
                    {
                        _selectedExploderType = ExploderType.Medium;
                    }

                    if (GUILayout.Button("Nuke"))
                    {
                        _selectedExploderType = ExploderType.Nuke;
                    }

                    GUILayout.Label("Selected Exploder Type: " + _selectedExploderType, EditorStyles.boldLabel);
                    break;
            }
        
            GUILayout.Label("");
        
            // Allows user to enter the attack speed attribute for the enemy
            GUILayout.Label("Enter Attack Speed", EditorStyles.boldLabel);
            _attackSpeed = EditorGUILayout.FloatField("Attack Speed:", _attackSpeed);

            GUILayout.Label("Enter Aggro Range", EditorStyles.boldLabel);
            _aggroRange = EditorGUILayout.FloatField("Attack Range:", _aggroRange);
            
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
            _enemyHP = EditorGUILayout.IntField("HP:", _enemyHP);

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
                    _spawnLocation = EditorGUILayout.Vector3Field("Spawn Location:", _spawnLocation);
                    // Handle spawning at specific position
                    break;

                case 2:
                    _spawnLocation = new Vector3(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100));
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
                        GUILayout.Label("WARNING: No GameObject selected to spawn on. Please select a GameObject to spawn on.");
                    }
                    break;
            }
            
            GUILayout.Label("");
        
            // Allows the user to enter the amount of enemies to spawn
            GUILayout.Label("Enter amount to Spawn", EditorStyles.boldLabel);
            _amountToSpawn = EditorGUILayout.IntField("Amount to Spawn:", _amountToSpawn);
        
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
                    EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
                    Health enemyHealth = newEnemy.GetComponent<Health>();
                    
                    enemyHealth.health = _enemyHP;
            
                    // Assign the attributes to the enemy
                    if (enemyScript != null)
                    {
                        //enemyScript.enemyAttributes = _enemyAttributes;
                        enemyScript.attackSpeed = _attackSpeed;
                        enemyScript.aggroRange = _aggroRange;
                        enemyScript.attackRange = _attackRange;
                        enemyScript.attackDamage = _attackDamage;
                        enemyScript.movementSpeed = _movementSpeed;

                        if (_selectedType == EnemyType.Melee)
                        {
                            // Find which weapon is selected, then get the corresponding child object and set it to active
                            if (_selectedMeleeWeapon == MeleeWeaponType.Longsword)
                            {
                                Transform childTransform = newEnemy.transform.GetChild(0);
        
                                // Checking if a child exists
                                if (childTransform != null)
                                {
                                    // Setting the child GameObject active
                                    childTransform.gameObject.SetActive(true);
                                }
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Greatsword)
                            {
                                Transform childTransform = newEnemy.transform.GetChild(1);
        
                                // Checking if a child exists
                                if (childTransform != null)
                                {
                                    // Setting the child GameObject active
                                    childTransform.gameObject.SetActive(true);
                                }
                            }
                            else if (_selectedMeleeWeapon == MeleeWeaponType.Shortsword)
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

    
    
