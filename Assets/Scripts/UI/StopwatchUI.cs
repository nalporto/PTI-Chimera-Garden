using UnityEngine;
using TMPro;

public class StopwatchUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stopwatchText;
    
    [Header("Settings")]
    [SerializeField] private bool showMilliseconds = true;
    [SerializeField] private Color runningColor = Color.white;
    [SerializeField] private Color finishedColor = Color.green;
    
    [Header("Debug")]
    [SerializeField] private bool isRunning = false;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private int enemiesRemaining = 0;
    
    public static StopwatchUI Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Always show stopwatch text with initial time
        if (stopwatchText != null)
        {
            stopwatchText.gameObject.SetActive(true);
            stopwatchText.color = runningColor;
            UpdateDisplay(); // Show 00:00.000 initially
        }
            
        CountEnemies();
    }
    
    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplay();
            CheckEnemiesStatus();
        }
    }
    
    public void StartStopwatch()
    {
        if (!isRunning)
        {
            isRunning = true;
            elapsedTime = 0f;
            
            // Change color to indicate running
            if (stopwatchText != null)
                stopwatchText.color = runningColor;
                
            CountEnemies();
        }
    }
    
    public void StopStopwatch()
    {
        if (isRunning)
        {
            isRunning = false;
            
            if (stopwatchText != null)
                stopwatchText.color = finishedColor;
        }
    }
    
    private void UpdateDisplay()
    {
        if (stopwatchText != null)
        {
            stopwatchText.text = FormatTime(elapsedTime);
        }
    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        
        if (showMilliseconds)
        {
            int milliseconds = Mathf.FloorToInt((time % 1f) * 1000f);
            return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    private void CountEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesRemaining = 0;
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<HitReceiver>() != null)
            {
                enemiesRemaining++;
            }
        }
    }
    
    private void CheckEnemiesStatus()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int activeEnemies = 0;
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<HitReceiver>() != null && enemy.activeInHierarchy)
            {
                activeEnemies++;
            }
        }
        
        if (activeEnemies == 0 && enemiesRemaining > 0)
        {
            StopStopwatch();
        }
        
        enemiesRemaining = activeEnemies;
    }
    
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }
}
