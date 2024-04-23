using System.Collections;
using AStarPathfinding;
using UnityEngine;

namespace EnemyUnits
{
    /// <summary>
    /// This class is responsible for the Melee Unit.
    /// It will subscribe to the event when the object is created and unsubscribe when the object is destroyed.
    /// It will also start the MeleeAttack Coroutine when the event is triggered
    /// </summary>
    public class MeleeUnit : MonoBehaviour
    {
        public float timeBetweenAttacks = 2f;
        private Animation _anim;
        private Pathfinding _pathfinding;
    
        /// <summary>
        /// Subscribes to the event when the object is created
        /// Gets the Animation component and Pathfinding component
        /// </summary>
        private void Start()
        {
            _anim = GetComponentInChildren<Animation>(); 
            _pathfinding = GetComponent<Pathfinding>();
            Pathfinding.Melee += DoTheMeleeAttack;
        }

        /// <summary>
        /// Unsubscribes from the event when the object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            Pathfinding.Melee -= DoTheMeleeAttack;
        }

        /// <summary>
        /// This method is called when the event is triggered.
        /// It will check if the source of the event is the same as the gameObject and if it is,
        /// it will start the attack Coroutine whilst the agent is also not attacking
        /// </summary>
        /// <param name="source">GameObject</param>
        private void DoTheMeleeAttack(GameObject source)
        {
            if (!_pathfinding.isAttacking && source == gameObject)
            {
                StartCoroutine(MeleeAttack());
            }
        }

        /// <summary>
        /// Coroutine that is called when the melee unit is attacking.
        /// It will play the attack animation and wait for the time between attacks
        /// </summary>
        /// <returns></returns>
        private IEnumerator MeleeAttack()
        {
            _pathfinding.isAttacking = true;
            _anim.Play();
            yield return new WaitForSeconds(timeBetweenAttacks);
            _pathfinding.isAttacking = false;
        
            yield return null;
        }
    }
}
