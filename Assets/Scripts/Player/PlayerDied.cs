using SupportingSystems;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Called when the player dies
    /// </summary>
    public class PlayerDied : BaseDie
    {
        /// <summary>
        /// Event that handles the player death event
        /// </summary>
        public static event KillCountChangedEvent OnPlayerDeath;
    
        /// <summary>
        /// Override the die method with specific logic for the player
        /// </summary>
        public override void Die()
        {
            Debug.Log("Died");
            OnPlayerDeath?.Invoke();
        }
    }
}