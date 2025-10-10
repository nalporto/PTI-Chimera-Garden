using UnityEngine;

public class GameClear : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float triggerDistance = 2f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color triggeredColor = Color.blue;
    
    private bool hasTriggered = false;
    private Renderer objectRenderer;
    private Color originalColor;
    
    void Start()
    {
        Debug.Log($"GameClear Start() - Object: {gameObject.name}, Position: {transform.position}");
        
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            objectRenderer.material.color = readyColor;
            Debug.Log("GameClear renderer found and set to ready color");
        }
        
        // Make sure we have a large trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
            Debug.Log($"GameClear collider set as trigger");
        }
    }
    
    void Update()
    {
        if (hasTriggered) return;
        
        // Check distance to player objects every frame
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        
        foreach (GameObject player in players)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                if (distance <= triggerDistance)
                {
                    Debug.Log($"Player '{player.name}' is within trigger distance ({distance:F2}m <= {triggerDistance}m) - triggering clear!");
                    TriggerGameClear();
                    return;
                }
            }
        }
    }
    
    // Keep collision detection as backup
    void OnCollisionEnter(Collision collision)
    {
        if (hasTriggered) return;
        
        Collider other = collision.collider;
        Debug.Log($"OnCollisionEnter: {other.name} (tag: {other.tag})");
        
        if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
        {
            Debug.Log("Player collision detected - triggering clear!");
            TriggerGameClear();
        }
    }
    
    // Keep trigger detection as backup  
    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        Debug.Log($"OnTriggerEnter: {other.name} (tag: {other.tag})");
        
        if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
        {
            Debug.Log("Player trigger detected - triggering clear!");
            TriggerGameClear();
        }
    }
    
    private void TriggerGameClear()
    {
        Debug.Log("=== TriggerGameClear() called! ===");
        hasTriggered = true;
        
        if (objectRenderer != null)
        {
            objectRenderer.material.color = triggeredColor;
        }
        
        Debug.Log("Checking ClearScreen.Instance...");
        if (ClearScreen.Instance != null)
        {
            Debug.Log("ClearScreen.Instance found! Getting stopwatch time...");
            float finalTime = 0f;
            if (StopwatchUI.Instance != null)
            {
                finalTime = StopwatchUI.Instance.GetElapsedTime();
                StopwatchUI.Instance.StopStopwatch();
                Debug.Log($"Stopwatch time: {finalTime} seconds");
            }
            else
            {
                Debug.LogWarning("StopwatchUI.Instance is null!");
            }
            
            Debug.Log("Calling ClearScreen.ShowClearScreen()...");
            ClearScreen.Instance.ShowClearScreen(finalTime);
            Debug.Log("ShowClearScreen() call completed!");
        }
        else
        {
            Debug.LogError("ClearScreen instance not found! Make sure ClearScreen script is in the scene.");
        }
    }
}