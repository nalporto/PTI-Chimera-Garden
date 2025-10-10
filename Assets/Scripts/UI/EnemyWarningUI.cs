using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyWarningUI : MonoBehaviour
{
    [Header("Warning UI Components")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Image warningBackground;
    
    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.8f);
    
    public static EnemyWarningUI Instance { get; private set; }
    
    private Coroutine currentWarningCoroutine;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (warningPanel != null)
                warningPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowWarning(int enemyCount)
    {
        if (currentWarningCoroutine != null)
        {
            StopCoroutine(currentWarningCoroutine);
        }
        
        currentWarningCoroutine = StartCoroutine(DisplayWarning(enemyCount));
    }
    
    private IEnumerator DisplayWarning(int enemyCount)
    {
        if (warningPanel == null || warningText == null) yield break;
        
        // Set warning message
        string message = enemyCount == 1 
            ? "1 enemy remains in the area!\nEliminate all enemies to complete the level."
            : $"{enemyCount} enemies remain in the area!\nEliminate all enemies to complete the level.";
        
        warningText.text = message;
        warningText.color = new Color(warningColor.r, warningColor.g, warningColor.b, 0f);
        
        if (warningBackground != null)
            warningBackground.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f);
        
        warningPanel.SetActive(true);
        
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            
            warningText.color = new Color(warningColor.r, warningColor.g, warningColor.b, alpha);
            
            if (warningBackground != null)
            {
                float bgAlpha = Mathf.Lerp(0f, backgroundColor.a, elapsedTime / fadeInDuration);
                warningBackground.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, bgAlpha);
            }
            
            yield return null;
        }
        
        // Full visibility
        warningText.color = warningColor;
        if (warningBackground != null)
            warningBackground.color = backgroundColor;
        
        // Wait for display duration
        yield return new WaitForSecondsRealtime(displayDuration);
        
        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            
            warningText.color = new Color(warningColor.r, warningColor.g, warningColor.b, alpha);
            
            if (warningBackground != null)
            {
                float bgAlpha = Mathf.Lerp(backgroundColor.a, 0f, elapsedTime / fadeOutDuration);
                warningBackground.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, bgAlpha);
            }
            
            yield return null;
        }
        
        // Hide panel
        warningPanel.SetActive(false);
        currentWarningCoroutine = null;
    }
}