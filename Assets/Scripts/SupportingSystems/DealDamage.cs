using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to deal damage
    /// </summary>
    public class DealDamage : MonoBehaviour
    {
        /// <summary>
        /// The attack damage of the item
        /// </summary>
        public int damage = 50;
        
        private bool _isAttacking;
    
        /// <summary>
        /// Used to start the attack.
        /// </summary>
        public void StartAttack()
        {
            _isAttacking = true;
        }
    
        /// <summary>
        /// Used to end the attack.
        /// </summary>
        public void EndAttack()
        {
            _isAttacking = false;
        }

        /// <summary>
        /// Handles the collision between the player and the enemy.
        /// </summary>
        /// <param name="other">GameObject that was collided with</param>
        private void OnTriggerEnter(Collider other)
        {
            // If the player is attacking and the enemy was hit whilst the current GameObject is the player
            if (_isAttacking && (other.gameObject.layer == 3 && gameObject.transform.parent.gameObject.layer == 9 || other.gameObject.layer == 9 && gameObject.layer == 3))
            {
                // Get the health component of the enemy
                Health health = other.GetComponentInParent<Health>();
                if (health != null)
                {
                    // Deal damage to the enemy
                    health.TakeDamage(damage);
                } 
            }
        }
    }
}
