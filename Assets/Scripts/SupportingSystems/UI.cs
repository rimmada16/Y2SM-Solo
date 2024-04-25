using EnemyUnits;
using Player;
using TMPro;
using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to update the UI and call the win event when the kill count reaches the amount of enemies
    /// </summary>
    public class UI : MonoBehaviour
    {
        /// <summary>
        /// Event for when the player reaches the amount of kills needed to run the game won logic
        /// </summary>
        public static event KillCountReachedEvent OnKillCountReached;
    
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI killCountText;
        
        private int _killCount = 0;
        private int _amountOfEnemies = 0;

        /// <summary>
        /// Subscribes to all the relevant events
        /// </summary>
        private void Start()
        {
            PlayerHealth.OnPlayerHealthChanged += UpdateHealthUI;
            EnemyDied.OnKillCountChanged += UpdateKillCount;
            ExploderUnit.OnExploderBlewItselfUp += UpdateKillCount;
            EnemyHealth.OnEnemySpawned += UpdateEnemyCount;
            PlayerHealth.OnPlayerInitialised += UpdateHealthUI;
        }

        /// <summary>
        /// Unsubscribes from all the relevant events
        /// </summary>
        private void OnDestroy()
        {
            PlayerHealth.OnPlayerHealthChanged -= UpdateHealthUI;
            EnemyDied.OnKillCountChanged -= UpdateKillCount;
            ExploderUnit.OnExploderBlewItselfUp -= UpdateKillCount;
            EnemyHealth.OnEnemySpawned -= UpdateEnemyCount;
            PlayerHealth.OnPlayerInitialised -= UpdateHealthUI;
        }

        /// <summary>
        /// Updates the health UI to the current health value
        /// </summary>
        /// <param name="player">The player with the health value</param>
        private void UpdateHealthUI(PlayerHealth player)
        {
            UpdateHealthUI(player.health);
        }

        /// <summary>
        /// Updates the health UI
        /// </summary>
        /// <param name="health">The current Health value</param>
        private void UpdateHealthUI(int health)
        {
            Debug.Log("Health has been updated to: " + health);
            healthText.text = "HP: " + health + " / 100";
        }
    
        /// <summary>
        /// Update the amount of enemies
        /// </summary>
        private void UpdateEnemyCount()
        {
            _amountOfEnemies++;
            UpdateKillCountUI();
            Debug.Log("Amount of enemies: " + _amountOfEnemies);
        }

        /// <summary>
        /// Update the kill count
        /// </summary>
        private void UpdateKillCount()
        {
            _killCount++;
            UpdateKillCountUI();
        
            // If the kill count equals the amount of enemies invoke the win event
            if (_killCount == _amountOfEnemies)
            {
                OnKillCountReached?.Invoke();
            }
        }
    
        /// <summary>
        /// Update the kill count UI
        /// </summary>
        private void UpdateKillCountUI()
        {
            killCountText.text = _killCount + " / " + _amountOfEnemies + " Enemies Killed";
        }
    }
}
