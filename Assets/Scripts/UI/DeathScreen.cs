using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DeathScreen : MonoBehaviour
{
    [Header("Death Screen Components")]
    [SerializeField] private GameObject deathScreenPanel;
    [SerializeField] private TextMeshProUGUI youDiedText;
    [SerializeField] private Image fadeImage;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float textFadeDelay = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Color textColor = Color.red;
    
    private static DeathScreen instance;
    public static DeathScreen Instance => instance;
    
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;
    private AudioSource musicAudioSource;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            deathScreenPanel.SetActive(false);
            
            // Find the MusicHandler AudioSource
            GameObject musicHandler = GameObject.Find("MusicHandler");
            if (musicHandler != null)
            {
                musicAudioSource = musicHandler.GetComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void TriggerDeathScreen()
    {
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Stop time
        Time.timeScale = 0f;
        
        // Store current cursor state and unlock mouse
        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable player controls
        DisablePlayerControls();
        
        // Stop the music
        if (musicAudioSource != null)
        {
            musicAudioSource.Pause();
        }
        
        deathScreenPanel.SetActive(true);
        
        youDiedText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 0.8f, elapsedTime / fadeInDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            
            if (elapsedTime >= textFadeDelay)
            {
                float textAlpha = Mathf.Lerp(0f, 1f, (elapsedTime - textFadeDelay) / (fadeInDuration - textFadeDelay));
                youDiedText.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
            }
            
            yield return null;
        }
        
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0.8f);
        youDiedText.color = textColor;
    }
    
    public void RestartGame()
    {
        // Restore time before changing scenes
        Time.timeScale = 1f;
        
        // Restore cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Load the Game scene
        SceneManager.LoadScene("Game");
    }
    
    private void DisablePlayerControls()
    {
        // Disable Player script (mouse look and input)
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = false;
        }
        
        // Disable all Shooter scripts
        Shooter[] shooters = FindObjectsOfType<Shooter>();
        foreach (Shooter shooter in shooters)
        {
            shooter.enabled = false;
        }
    }
    
    private void EnablePlayerControls()
    {
        // Re-enable Player script
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.enabled = true;
        }
        
        // Re-enable all Shooter scripts
        Shooter[] shooters = FindObjectsOfType<Shooter>();
        foreach (Shooter shooter in shooters)
        {
            shooter.enabled = true;
        }
    }
}
