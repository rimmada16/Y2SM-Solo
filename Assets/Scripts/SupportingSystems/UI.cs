using EnemyUnits;
using TMPro;
using UnityEngine;

namespace SupportingSystems
{
    /// <summary>
    /// This script is used to update the UI and call the win event when the kill count reaches the amount of enemies
    /// </summary>
    public class UI : MonoBehaviour
    {
        public static event KillCountReachedEvent OnKillCountReached;
    
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI killCountText;
        
        private int _killCount;
        private int _amountOfEnemies;

        /// <summary>
        /// Subscribes to all the relevant events
        /// </summary>
        private void Start()
        {
            Health.OnHealthValueChanged += UpdateHealthUI;
            Health.OnKillCountChanged += UpdateKillCount;
            ExploderUnit.OnExploderBlewItselfUp += UpdateKillCount;
            Health.OnKillCountInitialisation += UpdateKillCountUI;
            Health.OnEnemySpawned += UpdateEnemyCount;
        }

        /// <summary>
        /// Unsubscribes from all the relevant events
        /// </summary>
        private void OnDestroy()
        {
            Health.OnHealthValueChanged -= UpdateHealthUI;
            Health.OnKillCountChanged -= UpdateKillCount;
            ExploderUnit.OnExploderBlewItselfUp -= UpdateKillCount;
            Health.OnKillCountInitialisation -= UpdateKillCountUI;
            Health.OnEnemySpawned -= UpdateEnemyCount;
        }

        /// <summary>
        /// Updates the health UI depending on the damage taken
        /// </summary>
        /// <param name="health"></param>
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
