using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private int damage = 50;
    
    // Can deal damage by just walking into the sword need to make it so that isnt possible;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && gameObject.transform.parent.gameObject.layer == 9 || other.gameObject.layer == 9 && gameObject.layer == 3)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            } 
        }
    }
}
