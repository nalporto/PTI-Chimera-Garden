using UnityEngine;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private bool animateScale = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;
    
    private bool isVisible = false;
    private float targetAlpha = 0f;
    private Vector3 baseScale;
    
    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        baseScale = transform.localScale;
        canvasGroup.alpha = 0f;
    }
    
    void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        
        if (animateScale && isVisible)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = baseScale * pulse;
        }
        else
        {
            transform.localScale = baseScale;
        }
    }
    
    public void Show()
    {
        isVisible = true;
        targetAlpha = 1f;
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        isVisible = false;
        targetAlpha = 0f;
    }
    
    public void SetText(string text)
    {
        if (promptText != null)
        {
            promptText.text = text;
        }
    }
}
