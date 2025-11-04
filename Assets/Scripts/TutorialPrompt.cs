using UnityEngine;
using TMPro;

public class TutorialPrompt : MonoBehaviour
{
    [Header("Prompt Settings")]
    [SerializeField] private string promptMessage = "Press W, A, S, D to move";
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private bool triggerOnce = true;
    
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup promptCanvasGroup;
    
    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    private bool hasBeenTriggered = false;
    private float currentDisplayTime = 0f;
    private bool isDisplaying = false;
    private float fadeTimer = 0f;
    private enum FadeState { Idle, FadingIn, Displaying, FadingOut }
    private FadeState currentState = FadeState.Idle;

    private void Awake()
    {
        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasBeenTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            ShowPrompt();
            hasBeenTriggered = true;
        }
    }

    public void ShowPrompt()
    {
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
        
        currentState = FadeState.FadingIn;
        fadeTimer = 0f;
        isDisplaying = true;
    }

    private void Update()
    {
        if (!isDisplaying) return;

        switch (currentState)
        {
            case FadeState.FadingIn:
                fadeTimer += Time.deltaTime;
                float fadeInProgress = Mathf.Clamp01(fadeTimer / fadeInDuration);
                if (promptCanvasGroup != null)
                {
                    promptCanvasGroup.alpha = fadeInProgress;
                }
                
                if (fadeInProgress >= 1f)
                {
                    currentState = FadeState.Displaying;
                    currentDisplayTime = 0f;
                }
                break;

            case FadeState.Displaying:
                currentDisplayTime += Time.deltaTime;
                if (currentDisplayTime >= displayDuration)
                {
                    currentState = FadeState.FadingOut;
                    fadeTimer = 0f;
                }
                break;

            case FadeState.FadingOut:
                fadeTimer += Time.deltaTime;
                float fadeOutProgress = Mathf.Clamp01(fadeTimer / fadeOutDuration);
                if (promptCanvasGroup != null)
                {
                    promptCanvasGroup.alpha = 1f - fadeOutProgress;
                }
                
                if (fadeOutProgress >= 1f)
                {
                    currentState = FadeState.Idle;
                    isDisplaying = false;
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
    }
}
