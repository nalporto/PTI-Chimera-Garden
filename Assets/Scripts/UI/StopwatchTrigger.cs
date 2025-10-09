using UnityEngine;

public class StopwatchTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private bool oneTimeOnly = true;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visual Feedback")]
    [SerializeField] private Color triggerColor = Color.yellow;
    [SerializeField] private bool showTriggerZone = true;
    
    private bool hasTriggered = false;
    private Renderer objectRenderer;
    
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (triggerOnEnter && other.CompareTag(playerTag))
        {
            TriggerStopwatch();
        }
    }
    
    private void TriggerStopwatch()
    {
        if (oneTimeOnly && hasTriggered)
            return;
            
        if (StopwatchUI.Instance != null && !StopwatchUI.Instance.IsRunning())
        {
            StopwatchUI.Instance.StartStopwatch();
            hasTriggered = true;
            
            if (objectRenderer != null)
            {
                objectRenderer.material.color = triggerColor;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (showTriggerZone)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}
