namespace SupportingSystems
{
    /// <summary>
    /// This class is responsible for the death of an enemy.
    /// </summary>
    public class EnemyDied : BaseDie
    {
        /// <summary>
        /// Used to kill an enemy
        /// </summary>
        public bool test = false; 
        
        /// <summary>
        /// Test function to check the death of an enemy
        /// </summary>
        private void Update()
        {
            if (test)
            {
                test = !test;
                Die();
            }
        }

        /// <summary>
        /// Event to update the kill count when an enemy is slain
        /// </summary>
        public static event KillCountChangedEvent OnKillCountChanged;
    
        /// <summary>
        /// Override the die method with specific logic for the enemy
        /// </summary>
        public override void Die()
        {
            OnKillCountChanged?.Invoke();
        
            DeleteSelf();
        }
    }
}