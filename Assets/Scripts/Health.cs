using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        health += healAmount;
    }
    
    private void Die()
    {
        Debug.Log(this +  "Game Object has died!");
        Destroy(gameObject);
    }
    
}
