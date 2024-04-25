using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace SupportingSystems
{
    /// <summary>
    /// Base class for health management
    /// </summary>
    public class Health : MonoBehaviour, IDamageable, IHealable
    {
        /// <summary>
        /// Event that is called when the health value changes
        /// </summary>
        protected event HealthChanged OnHealthChanged;
        
        /// <summary>
        /// Handles the testing of the damage system
        /// </summary>
        public bool damageUnitTest = false;
        
        /// <summary>
        /// The health value of the entity
        /// </summary>
        public int health = 100;
        
        private IDie _die;

        /// <summary>
        /// Alters the health of the unit for testing purposes
        /// </summary>
        private void Update()
        {
            if (damageUnitTest)
            {
                damageUnitTest = !damageUnitTest;
                TakeDamage(10);
            }
        }

        /// <summary>
        /// Gets the IDie component
        /// </summary>
        protected virtual void Awake()
        {
            _die = GetComponent<IDie>();
        }

        /// <summary>
        /// On start, if the game object is a player, invoke the OnHealthValueChanged event to update the UI
        /// if the game object is an enemy, invoke the OnEnemySpawned event to update the UI for enemy count
        /// </summary>
        protected virtual void Start()
        {
            
        }

        /// <summary>
        /// This method is used to apply damage
        /// </summary>
        /// <param name="damage">Int that is used to deduce how much damage to apply</param>
        public void TakeDamage(int damage)
        {
            var damageTaken = Mathf.Min(health, damage);
            
            health -= damageTaken;
        
            // if damage taken invoke event
            OnHealthChanged?.Invoke(health);

            if (health <= 0)
            {
                _die?.Die();
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
    }
}