using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject _player;
    private float _movementSpeed = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        EnemyScript enemyScript = GetComponent<EnemyScript>();
        _movementSpeed = enemyScript.movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
