using System;
using System.Collections;
using System.Collections.Generic;
using AStarPathfinding;
using UnityEngine;

public class ArcherUnit : MonoBehaviour
{

    private Pathfinding _pathfinding;
    public int arrowToShoot = 0;
    public float timeBetweenAttacks = 2f;
    
    // Start is called before the first frame update
    private void Start()
    {
        _pathfinding = GetComponent<Pathfinding>();
        Pathfinding.Ranged += DoTheRangedAttack;    
    }

    private void OnDestroy()
    {
        Pathfinding.Ranged -= DoTheRangedAttack;
    }

    private void DoTheRangedAttack(GameObject source)
    {
        if (!_pathfinding.isAttacking && source == gameObject)
        {
            StartCoroutine(BowAttack());
        }
    }
    
    private IEnumerator BowAttack()
    {
        _pathfinding.isAttacking = true;
        ProjectileManager.Instance.FireProjectile(arrowToShoot, transform.position, transform.forward, gameObject);
        yield return new WaitForSeconds(timeBetweenAttacks);
        _pathfinding.isAttacking = false;
        
        yield return null;
    }
}
