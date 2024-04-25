using SupportingSystems;

namespace Player
{
    /// <summary>
    /// Manages the health of the player
    /// </summary>
    public class PlayerHealth : Health
    {
        /// <summary>
        /// Event delegate that sends the specific health value of the player
        /// </summary>
        public delegate void PlayerEvent(PlayerHealth player);
        
        /// <summary>
        /// Event that is called when the players health value changes
        /// </summary>
        public static event HealthChanged OnPlayerHealthChanged;

        /// <summary>
        /// Event that is called on the game start to initialise the player
        /// </summary>
        public static event PlayerEvent OnPlayerInitialised;

        /// <summary>
        /// Call the base and subscribe to the relevant event
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            OnHealthChanged += (i) => OnPlayerHealthChanged?.Invoke(i);
        }

        /// <summary>
        /// Call the base and initialise the player
        /// </summary>
        protected override void Start()
        {
            base.Start();
            OnPlayerInitialised?.Invoke(this);
        }
    }
}