using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public int damage = 50;
    private bool _isAttacking;
    
    public void StartAttack()
    {
        _isAttacking = true;
    }
    
    public void EndAttack()
    {
        _isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAttacking && (other.gameObject.layer == 3 && gameObject.transform.parent.gameObject.layer == 9 || other.gameObject.layer == 9 && gameObject.layer == 3))
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            } 
        }
    }
}
