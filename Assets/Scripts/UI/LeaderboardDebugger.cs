using UnityEngine;

public class LeaderboardDebugger : MonoBehaviour
{
    [Header("Debug Actions")]
    [SerializeField] private bool clearLeaderboardOnStart = false;
    
    [Header("Test Times")]
    [SerializeField] private float testTime1 = 65.5f;
    [SerializeField] private float testTime2 = 72.3f;
    [SerializeField] private float testTime3 = 80.1f;
    
    void Start()
    {
        if (clearLeaderboardOnStart)
        {
            ClearLeaderboard();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftShift))
        {
            ClearLeaderboard();
        }
        
        if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftShift))
        {
            AddTestTimes();
        }
    }
    
    public void ClearLeaderboard()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.ClearLeaderboard();
            Debug.Log("Leaderboard cleared!");
            
            LeaderboardUI ui = FindObjectOfType<LeaderboardUI>();
            if (ui != null)
            {
                ui.UpdateLeaderboardDisplay();
                Debug.Log("Leaderboard UI updated!");
            }
        }
        else
        {
            Debug.LogWarning("LeaderboardManager.Instance is null!");
        }
    }
    
    public void AddTestTimes()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.TryAddTime(testTime1);
            LeaderboardManager.Instance.TryAddTime(testTime2);
            LeaderboardManager.Instance.TryAddTime(testTime3);
            Debug.Log($"Added test times: {testTime1}, {testTime2}, {testTime3}");
            
            LeaderboardUI ui = FindObjectOfType<LeaderboardUI>();
            if (ui != null)
            {
                ui.UpdateLeaderboardDisplay();
                Debug.Log("Leaderboard UI updated with test times!");
            }
        }
        else
        {
            Debug.LogWarning("LeaderboardManager.Instance is null!");
        }
    }
    
    public void ShowCurrentLeaderboard()
    {
        if (LeaderboardManager.Instance != null)
        {
            var topTimes = LeaderboardManager.Instance.GetTopTimes();
            Debug.Log("=== CURRENT LEADERBOARD ===");
            for (int i = 0; i < topTimes.Count; i++)
            {
                string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : "🥉";
                Debug.Log($"{medal} #{i + 1}: {FormatTime(topTimes[i])}");
            }
            if (topTimes.Count == 0)
            {
                Debug.Log("Leaderboard is empty");
            }
        }
    }
    
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time % 1f) * 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
