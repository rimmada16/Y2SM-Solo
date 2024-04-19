using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public delegate void ExploderEvent(GameObject source);

public delegate void MeleeAttackEvent(GameObject source);

public delegate void RangedAttackEvent(GameObject source);

public delegate void HealthValueChangedEvent(int health);

public delegate void KillCountChangedEvent();

public delegate void KillCountInitialisationEvent();

public delegate void EnemySpawnedEvent();

public delegate void KillCountReachedEvent();

public delegate void PlayerDeathEvent();


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool _isPaused;
    private bool _isWinCanvasActive;
    private bool _isPlayerDeathCanvasActive;
    [SerializeField] private GameObject defaultCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject playerDeathCanvas;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        Time.timeScale = 1;
        UI.OnKillCountReached += WinCondition;
        Health.OnPlayerDeath += PlayerDeath;
    }

    private void OnDestroy()
    {
        UI.OnKillCountReached -= WinCondition;
        Health.OnPlayerDeath -= PlayerDeath;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Main Menu" && !_isWinCanvasActive && !_isPlayerDeathCanvasActive)
        {
            if (_isPaused)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isPaused = true;
        defaultCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }
    
    public void UnPauseGame()
    {
        Time.timeScale = 1;
        _isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        defaultCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }

    private void WinCondition()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isWinCanvasActive = true;
        winCanvas.SetActive(true);
    }
    
    private void PlayerDeath()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isPlayerDeathCanvasActive = true;
        defaultCanvas.SetActive(false);
        playerDeathCanvas.SetActive(true);
    }

    public void Restart()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("Main Menu");
    }
    
    public void LoadLevelOne()
    {
        Debug.Log("Loading Level One...");
        SceneManager.LoadScene("Level One");
    }

    public void LoadLevelTwo()
    {
        Debug.Log("Loading Level Two...");
        SceneManager.LoadScene("Level Two");
    }

    public void LoadLevelThree()
    {
        Debug.Log("Loading Level Three...");
        SceneManager.LoadScene("Level Three");
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit the Game...");
        Application.Quit();
    }
}
