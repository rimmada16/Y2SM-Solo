using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void ExploderEvent(GameObject source);

public delegate void MeleeAttackEvent(GameObject source);

public delegate void RangedAttackEvent(GameObject source);

public delegate void HealthValueChangedEvent(int health);

public delegate void KillCountChangedEvent();

public delegate void KillCountInitialisationEvent();

public delegate void EnemySpawnedEvent();


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
