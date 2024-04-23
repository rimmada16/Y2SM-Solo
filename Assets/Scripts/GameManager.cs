using SupportingSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Event delegate for when an exploder initiates its attack
/// </summary>
public delegate void ExploderEvent(GameObject source);

/// <summary>
/// Event delegate for when a melee unit initiates its attack
/// </summary>
public delegate void MeleeAttackEvent(GameObject source);

/// <summary>
/// Event delegate for when an archer initiates its attack
/// </summary>
public delegate void RangedAttackEvent(GameObject source);

/// <summary>
/// Event delegate for when a the players health changes
/// </summary>
public delegate void HealthValueChangedEvent(int health);

/// <summary>
/// Event delegate for when the player kills an enemy
/// </summary>
public delegate void KillCountChangedEvent();

/// <summary>
/// Event delegate for when an exploder kills itself
/// </summary>
public delegate void ExploderBlewItselfUpEvent();

/// <summary>
/// Event delegate to initialise the kill count
/// </summary>
public delegate void KillCountInitialisationEvent();

/// <summary>
/// Event delegate to initialise the amount of enemies
/// </summary>
public delegate void EnemySpawnedEvent();

/// <summary>
/// Event delegate for when the player reaches the kill count - win condition
/// </summary>
public delegate void KillCountReachedEvent();

/// <summary>
/// Event delegate for when the player dies
/// </summary>
public delegate void PlayerDeathEvent();

/// <summary>
/// The GameManager class is a singleton class that manages the game state
/// </summary>
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

    /// <summary>
    /// Ensures that there is only one instance of the GameManager
    /// Sets the timescale to 1
    /// Subscribe to the relevant events
    /// </summary>
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

    /// <summary>
    /// Unsubscribe from the relevant events
    /// </summary>
    private void OnDestroy()
    {
        UI.OnKillCountReached -= WinCondition;
        Health.OnPlayerDeath -= PlayerDeath;
    }

    /// <summary>
    /// Restart the game if the player presses the 'R' key
    /// If the player presses the 'Escape' key, pause the game. If the game is already paused, unpause the game
    /// While the active scene is not the 'Main Menu' scene,
    /// the win canvas is not active and the player death canvas is not active
    /// </summary>
    private void Update()
    {
        // Restart the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        
        // Pause & Unpause the game
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

    /// <summary>
    /// Handles the pausing of the game
    /// Locks the cursor, enables the pause menu canvas and disables the default canvas
    /// Sets the timescale to 0 and sets the _isPaused boolean to true
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isPaused = true;
        defaultCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }
    
    /// <summary>
    /// Handles the unpausing of the game
    /// Unlocks the cursor, disables the pause menu canvas and enables the default canvas
    /// Sets the timescale to 1 and sets the _isPaused boolean to false
    /// </summary>
    public void UnPauseGame()
    {
        Time.timeScale = 1;
        _isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        defaultCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }

    /// <summary>
    /// Handles the win condition of the game
    /// Sets the timescale to 0, unlocks the cursor and enables the win canvas
    /// </summary>
    private void WinCondition()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isWinCanvasActive = true;
        winCanvas.SetActive(true);
    }
    
    /// <summary>
    /// Handles the player death condition
    /// Sets the timescale to 0, unlocks the cursor and enables the player death canvas
    /// </summary>
    public void PlayerDeath()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        _isPlayerDeathCanvasActive = true;
        defaultCanvas.SetActive(false);
        playerDeathCanvas.SetActive(true);
    }

    /// <summary>
    /// Gets the current scene name
    /// Then reloads the current scene
    /// </summary>
    public void Restart()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    /// <summary>
    /// Changes the scene to the 'Main Menu' scene
    /// </summary>
    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("Main Menu");
    }
    
    /// <summary>
    /// Changes the scene to the 'Level One' scene
    /// </summary>
    public void LoadLevelOne()
    {
        Debug.Log("Loading Level One...");
        SceneManager.LoadScene("Level One");
    }

    /// <summary>
    /// Changes the scene to the 'Level Two' scene
    /// </summary>
    public void LoadLevelTwo()
    {
        Debug.Log("Loading Level Two...");
        SceneManager.LoadScene("Level Two");
    }

    /// <summary>
    /// Changes the scene to the 'Level Three' scene
    /// </summary>
    public void LoadLevelThree()
    {
        Debug.Log("Loading Level Three...");
        SceneManager.LoadScene("Level Three");
    }
    
    /// <summary>
    /// Quits the game - ALT F4 Equivalent
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit the Game...");
        Application.Quit();
    }
}
