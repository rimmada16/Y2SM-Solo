using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Health : MonoBehaviour, IDamageable, IHealable
{ 
    public static event HealthValueChangedEvent OnHealthValueChanged;
    public static event KillCountChangedEvent OnKillCountChanged;
    public static event KillCountInitialisationEvent OnKillCountInitialisation;
    public static event EnemySpawnedEvent OnEnemySpawned;
    public static event PlayerDeathEvent OnPlayerDeath;
    
    public int health = 100;

    private void Start()
    {
        if (gameObject.layer == 9)
        {
            OnHealthValueChanged?.Invoke(health);
        }
        if (gameObject.layer == 3)
        {
            OnEnemySpawned?.Invoke();
            OnKillCountInitialisation?.Invoke();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (gameObject.layer == 9)
        {
            OnHealthValueChanged?.Invoke(health);
        }
        
        if (gameObject.layer != 9 && health <= 0)
        {
            Die();
        }
        else if (gameObject.layer == 9 && health <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }
    
    public void Heal(int healAmount)
    {
        health += healAmount;
    }
    
    private void Die()
    {
        Debug.Log(this +  "Game Object has died!");
        if (gameObject.layer == 3)
        {
            OnKillCountChanged?.Invoke();
        }
        Destroy(gameObject);
    }
}
