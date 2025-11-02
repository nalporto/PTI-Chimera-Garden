using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ClearScreen : MonoBehaviour
{
    [Header("Clear Screen Components")]
    [SerializeField] private GameObject clearScreenPanel;
    [SerializeField] private TextMeshProUGUI congratulationsText;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image fadeImage;
    [SerializeField] private LeaderboardUI leaderboardUI;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float textFadeDelay = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Color congratsColor = Color.white;
    [SerializeField] private Color timeColor = Color.white;
    [SerializeField] private Color newRecordColor = Color.yellow;
    
    public static ClearScreen Instance { get; private set; }
    
    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;
    private AudioSource musicAudioSource;
    
    void Update()
    {
        if (clearScreenPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Keyboard shortcut pressed - restarting!");
                RestartGame();
            }
        }
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            clearScreenPanel.SetActive(false);
            
            GameObject musicHandler = GameObject.Find("MusicHandler");
            if (musicHandler != null)
            {
                musicAudioSource = musicHandler.GetComponent<AudioSource>();
            }
            
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
                Debug.Log("RestartGame listener added to button!");
            }
            else
            {
                Debug.LogError("RestartButton is null in ClearScreen Awake!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowClearScreen(float completionTime)
    {
        Debug.Log($"ClearScreen.ShowClearScreen() called with time: {completionTime}");
        
        if (clearScreenPanel == null)
        {
            Debug.LogError("clearScreenPanel is null! Please assign it in the Inspector.");
            return;
        }
        
        Debug.Log("Starting ClearSequence coroutine...");
        StartCoroutine(ClearSequence(completionTime));
    }
    
    private IEnumerator ClearSequence(float completionTime)
    {
        Debug.Log("ClearSequence coroutine started!");
        
        Time.timeScale = 0f;
        Debug.Log("Time scale set to 0");
        
        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor state updated");
        
        DisablePlayerControls();
        Debug.Log("Player controls disabled");
        
        if (musicAudioSource != null)
        {
            musicAudioSource.Pause();
            Debug.Log("Music paused");
        }
        
        clearScreenPanel.SetActive(true);
        Debug.Log("Clear screen panel activated!");
        
        congratulationsText.color = new Color(congratsColor.r, congratsColor.g, congratsColor.b, 0f);
        finalTimeText.color = new Color(timeColor.r, timeColor.g, timeColor.b, 0f);
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        
        bool isNewRecord = false;
        int rank = -1;
        
        if (LeaderboardManager.Instance != null)
        {
            isNewRecord = LeaderboardManager.Instance.TryAddTime(completionTime);
            rank = LeaderboardManager.Instance.GetRank(completionTime);
            
            if (rank == 1)
            {
                congratulationsText.text = "🥇 NEW GOLD RECORD!";
                congratsColor = newRecordColor;
            }
            else if (rank == 2)
            {
                congratulationsText.text = "🥈 NEW SILVER RECORD!";
                congratsColor = newRecordColor;
            }
            else if (rank == 3)
            {
                congratulationsText.text = "🥉 NEW BRONZE RECORD!";
                congratsColor = newRecordColor;
            }
            else
            {
                congratulationsText.text = "LEVEL COMPLETE!";
            }
        }
        else
        {
            congratulationsText.text = "LEVEL COMPLETE!";
        }
        
        finalTimeText.text = "Time: " + FormatTime(completionTime);
        Debug.Log($"Text set - Congratulations: '{congratulationsText.text}', Time: '{finalTimeText.text}'");
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.interactable = false;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 0.8f, elapsedTime / fadeInDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            
            if (elapsedTime >= textFadeDelay)
            {
                float textProgress = (elapsedTime - textFadeDelay) / (fadeInDuration - textFadeDelay);
                float textAlpha = Mathf.Lerp(0f, 1f, textProgress);
                congratulationsText.color = new Color(congratsColor.r, congratsColor.g, congratsColor.b, textAlpha);
                finalTimeText.color = new Color(timeColor.r, timeColor.g, timeColor.b, textAlpha);
            }
            
            yield return null;
        }
        
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0.8f);
        congratulationsText.color = congratsColor;
        finalTimeText.color = timeColor;
        
        if (leaderboardUI != null)
        {
            leaderboardUI.UpdateLeaderboardDisplay();
        }
        
        yield return new WaitForSecondsRealtime(1f);
        
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.interactable = true;
            Debug.Log("Restart button activated and set to interactable!");
        }
    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
    
    public void RestartGame()
    {
        Debug.Log("RestartGame() called!");
        
        StopAllCoroutines();
        
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Loading scene 'Game'...");
        SceneManager.LoadScene("Game");
    }
    
    private void DisablePlayerControls()
    {
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
}