using Interfaces;
using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to handle the health of the player and enemies
    /// </summary>
    public class Health : MonoBehaviour, IDamageable, IHealable
    { 
        // Events
        public static event HealthValueChangedEvent OnHealthValueChanged;
        public static event KillCountChangedEvent OnKillCountChanged;
        public static event KillCountInitialisationEvent OnKillCountInitialisation;
        public static event EnemySpawnedEvent OnEnemySpawned;
        public static event PlayerDeathEvent OnPlayerDeath;
    
        public int health = 100;

        /// <summary>
        /// On start, if the game object is a player, invoke the OnHealthValueChanged event to update the UI
        /// if the game object is an enemy, invoke the OnEnemySpawned event to update the UI for enemy count
        /// </summary>
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

        /// <summary>
        /// This method is used to apply damage
        /// </summary>
        /// <param name="damage">Int that is used to deduce how much damage to apply</param>
        public void TakeDamage(int damage)
        {
            health -= damage;
        
            // Update the health UI if the game object is a player
            if (gameObject.layer == 9)
            {
                OnHealthValueChanged?.Invoke(health);
            }
        
            // Call the Die method if the GameObject is the player and the health is less than or equal to 0
            if (gameObject.layer != 9 && health <= 0)
            {
                Die();
            }
            // Invoke the player death event if the GameObject is the player and the health is less than or equal to 0
            else if (gameObject.layer == 9 && health <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
        }
    
        /// <summary>
        /// Method used to heal the player
        /// </summary>
        /// <param name="healAmount">Int that is used to deduce how much health to health to add</param>
        public void Heal(int healAmount)
        {
            health += healAmount;
        }
    
        /// <summary>
        /// Invoke the OnKillCountChanged event if on the enemy layer and destroy the GameObject
        /// </summary>
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
}
