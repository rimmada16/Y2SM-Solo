using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static event KillCountReachedEvent OnKillCountReached;
    
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI killCountText;
    private int _killCount;
    private int _amountOfEnemies;

    // Start is called before the first frame update
    private void Start()
    {
        Health.OnHealthValueChanged += UpdateHealthUI;
        Health.OnKillCountChanged += UpdateKillCount;
        Health.OnKillCountInitialisation += UpdateKillCountUI;
        Health.OnEnemySpawned += UpdateEnemyCount;
    }

    private void OnDestroy()
    {
        Health.OnHealthValueChanged -= UpdateHealthUI;
        Health.OnKillCountChanged -= UpdateKillCount;
        Health.OnKillCountInitialisation -= UpdateKillCountUI;
        Health.OnEnemySpawned -= UpdateEnemyCount;
    }

    private void UpdateHealthUI(int health)
    {
        Debug.Log("Health has been updated to: " + health);
        healthText.text = "HP: " + health + " / 100";
    }
    
    private void UpdateEnemyCount()
    {
        _amountOfEnemies++;
        Debug.Log("Amount of enemies: " + _amountOfEnemies);
    }

    private void UpdateKillCount()
    {
        _killCount++;
        UpdateKillCountUI();
        
        if (_killCount == _amountOfEnemies)
        {
            OnKillCountReached?.Invoke();
        }
    }
    
    private void UpdateKillCountUI()
    {
        killCountText.text = _killCount + " / " + _amountOfEnemies + " Enemies Killed";

    }
    
    // If kill count equals the enemy count invoke an event for the game manager to handle the win event
}
