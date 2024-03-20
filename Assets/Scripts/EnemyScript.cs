using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //[SerializeField] public SpawnManagerScriptableObject enemyAttributes;

    public float attackSpeed;
    public float aggroRange;
    public float attackRange;
    public float attackDamage;
    public float movementSpeed;
    private bool inAttackRange;
    
    public GameObject _player;
    private Rigidbody enemyRb;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        enemyRb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(attackSpeed);
            Debug.Log(attackRange);
            Debug.Log(attackDamage);
            Debug.Log(movementSpeed);
        }
        
        /*if (Vector3.Distance(transform.position, _player.transform.position) < aggroRange && !inAttackRange)
        {
            Vector3 playerVector = _player.transform.position;
            
            //Rigidbody.MovePosition(playerVector);
            
            
            
            // Move towards player
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, movementSpeed * Time.deltaTime);
            inAttackRange = false;
        }*/
        
        if (Vector3.Distance(transform.position, _player.transform.position) < attackRange)
        {
            // Attack
            inAttackRange = true;
            
            // Attack Logic
        }
        else
        {
            inAttackRange = false;
        }
        
        
    }
}
