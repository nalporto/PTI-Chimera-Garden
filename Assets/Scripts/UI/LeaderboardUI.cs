using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard Text References")]
    [SerializeField] private TextMeshProUGUI goldTimeText;
    [SerializeField] private TextMeshProUGUI silverTimeText;
    [SerializeField] private TextMeshProUGUI bronzeTimeText;
    
    [Header("Colors")]
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color emptyColor = new Color(0.5f, 0.5f, 0.5f);
    
    [Header("Display Format")]
    [SerializeField] private string goldPrefix = "🥇 ";
    [SerializeField] private string silverPrefix = "🥈 ";
    [SerializeField] private string bronzePrefix = "🥉 ";
    [SerializeField] private string emptyText = "--:--:---";
    
    void Start()
    {
        UpdateLeaderboardDisplay();
    }
    
    public void UpdateLeaderboardDisplay()
    {
        if (LeaderboardManager.Instance == null)
        {
            Debug.LogWarning("LeaderboardManager.Instance is null!");
            return;
        }
        
        List<float> topTimes = LeaderboardManager.Instance.GetTopTimes();
        
        if (goldTimeText != null)
        {
            if (topTimes.Count > 0)
            {
                goldTimeText.text = goldPrefix + FormatTime(topTimes[0]);
                goldTimeText.color = goldColor;
            }
            else
            {
                goldTimeText.text = goldPrefix + emptyText;
                goldTimeText.color = emptyColor;
            }
        }
        
        if (silverTimeText != null)
        {
            if (topTimes.Count > 1)
            {
                silverTimeText.text = silverPrefix + FormatTime(topTimes[1]);
                silverTimeText.color = silverColor;
            }
            else
            {
                silverTimeText.text = silverPrefix + emptyText;
                silverTimeText.color = emptyColor;
            }
        }
        
        if (bronzeTimeText != null)
        {
            if (topTimes.Count > 2)
            {
                bronzeTimeText.text = bronzePrefix + FormatTime(topTimes[2]);
                bronzeTimeText.color = bronzeColor;
            }
            else
            {
                bronzeTimeText.text = bronzePrefix + emptyText;
                bronzeTimeText.color = emptyColor;
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
