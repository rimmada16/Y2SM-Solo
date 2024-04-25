namespace SupportingSystems
{
    /// <summary>
    /// Class that handles the health of enemies
    /// </summary>
    public class EnemyHealth : Health
    {
        /// <summary>
        /// Event to initialise the amount of enemies that need to be killed on the UI
        /// </summary>
        public static event EnemySpawnedEvent OnEnemySpawned;

        /// <summary>
        /// Inform the UI that an enemy has spawned
        /// </summary>
        protected override void Start()
        {
            OnEnemySpawned?.Invoke();
        }
    }
}