using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;
    
    [Header("Game State")]
    [SerializeField] private bool isGamePaused = false;
    [SerializeField] private bool playerIsDead = false;
    
    public bool IsGamePaused => isGamePaused;
    public bool PlayerIsDead => playerIsDead;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetPlayerDead(bool isDead)
    {
        playerIsDead = isDead;
        
        if (isDead)
        {
            PauseGame();
            DisablePlayerControls();
        }
        else
        {
            ResumeGame();
            EnablePlayerControls();
        }
    }
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    private void DisablePlayerControls()
    {
        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player input and shooting
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = false;
        }
        
        Shooter[] shooters = FindObjectsOfType<Shooter>();
        foreach (Shooter shooter in shooters)
        {
            shooter.enabled = false;
        }
    }
    
    private void EnablePlayerControls()
    {
        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable player input and shooting
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = true;
        }
        
        Shooter[] shooters = FindObjectsOfType<Shooter>();
        foreach (Shooter shooter in shooters)
        {
            shooter.enabled = true;
        }
    }
    
    void Update()
    {
        // Optional: Add debug controls
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
}