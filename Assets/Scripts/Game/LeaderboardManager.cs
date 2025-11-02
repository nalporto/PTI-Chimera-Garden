using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    private const string LEADERBOARD_KEY_PREFIX = "Leaderboard_Time_";
    private const int MAX_ENTRIES = 3;
    
    public static LeaderboardManager Instance { get; private set; }
    
    private List<float> topTimes = new List<float>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadLeaderboard()
    {
        topTimes.Clear();
        
        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            string key = LEADERBOARD_KEY_PREFIX + i;
            if (PlayerPrefs.HasKey(key))
            {
                float time = PlayerPrefs.GetFloat(key);
                topTimes.Add(time);
            }
        }
        
        topTimes = topTimes.OrderBy(t => t).ToList();
    }
    
    public void SaveLeaderboard()
    {
        for (int i = 0; i < topTimes.Count && i < MAX_ENTRIES; i++)
        {
            string key = LEADERBOARD_KEY_PREFIX + i;
            PlayerPrefs.SetFloat(key, topTimes[i]);
        }
        
        PlayerPrefs.Save();
    }
    
    public bool TryAddTime(float newTime)
    {
        if (topTimes.Count < MAX_ENTRIES)
        {
            topTimes.Add(newTime);
            topTimes = topTimes.OrderBy(t => t).ToList();
            SaveLeaderboard();
            return true;
        }
        else if (newTime < topTimes[MAX_ENTRIES - 1])
        {
            topTimes[MAX_ENTRIES - 1] = newTime;
            topTimes = topTimes.OrderBy(t => t).ToList();
            SaveLeaderboard();
            return true;
        }
        
        return false;
    }
    
    public List<float> GetTopTimes()
    {
        return new List<float>(topTimes);
    }
    
    public int GetRank(float time)
    {
        for (int i = 0; i < topTimes.Count; i++)
        {
            if (Mathf.Approximately(topTimes[i], time))
            {
                return i + 1;
            }
        }
        return -1;
    }
    
    public void ClearLeaderboard()
    {
        topTimes.Clear();
        
        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            string key = LEADERBOARD_KEY_PREFIX + i;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        
        PlayerPrefs.Save();
    }
}
