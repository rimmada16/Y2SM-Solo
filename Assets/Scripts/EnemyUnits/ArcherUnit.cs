using System.Collections;
using AStarPathfinding;
using ProjectileSystem;
using UnityEngine;

namespace EnemyUnits
{
    /// <summary>
    /// This class is responsible for the Archer Unit.
    /// It will subscribe to the event when the object is created and unsubscribe when the object is destroyed.
    /// It will also start the BowAttack Coroutine when the event is triggered
    /// </summary>
    public class ArcherUnit : MonoBehaviour
    {
        private Pathfinding _pathfinding;
        public int arrowToShoot = 0;
        public float timeBetweenAttacks = 2f;
    
        /// <summary>
        /// Subscribes to the event when the object is created
        /// </summary>
        private void Start()
        {
            _pathfinding = GetComponent<Pathfinding>();
            Pathfinding.Ranged += DoTheRangedAttack;    
        }

        /// <summary>
        /// Unsubscribes from the event when the object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            Pathfinding.Ranged -= DoTheRangedAttack;
        }

        /// <summary>
        /// This method is called when the event is triggered.
        /// It will check if the source of the event is the same as the gameObject and if it is,
        /// it will start the attack Coroutine
        /// </summary>
        /// <param name="source">GameObject</param>
        private void DoTheRangedAttack(GameObject source)
        {
            if (!_pathfinding.isAttacking && source == gameObject)
            {
                StartCoroutine(BowAttack());
            }
        }
    
        /// <summary>
        /// This Coroutine is called when the archer unit is attacking. It will fire a projectile and wait for the time between attacks
        /// </summary>
        /// <returns></returns>
        private IEnumerator BowAttack()
        {
            _pathfinding.isAttacking = true;
            ProjectileManager.Instance.FireProjectile(arrowToShoot, transform.position, transform.forward, gameObject);
            yield return new WaitForSeconds(timeBetweenAttacks);
            _pathfinding.isAttacking = false;
        
            yield return null;
        }
    }
}
