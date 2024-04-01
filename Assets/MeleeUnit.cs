using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarPathfinding;

public class MeleeUnit : MonoBehaviour
{
    private Animation _anim;
    public float timeBetweenAttacks = 2f;
    private Pathfinding _pathfinding;
    
    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponentInChildren<Animation>(); 
        _pathfinding = GetComponent<Pathfinding>();
        Pathfinding.Melee += DoTheMeleeAttack;
    }

    private void OnDestroy()
    {
        Pathfinding.Melee -= DoTheMeleeAttack;
    }

    private void DoTheMeleeAttack(GameObject source)
    {
        if (!_pathfinding.isAttacking && source == gameObject)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    private IEnumerator MeleeAttack()
    {
        _pathfinding.isAttacking = true;
        _anim.Play();
        // Wait amount should be dependant on the weapon type used.
        // Currently is not
        yield return new WaitForSeconds(timeBetweenAttacks);
        _pathfinding.isAttacking = false;
        
        yield return null;
    }

}
