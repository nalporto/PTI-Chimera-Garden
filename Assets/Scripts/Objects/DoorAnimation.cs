using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator;
    public float interactionDistance = 3f;
    public GameObject player;
    public Transform interactionPoint;

    [Header("Delay")]
    [Tooltip("Seconds the player must be inside interactionDistance before the door will open.")]
    public float openDelay = 0.0f;
    
    [Header("Lock Settings")]
    [Tooltip("If true, door will not open until manually unlocked")]
    public bool isLocked = false;

    private float timeInRange = 0f;
    private bool hasBeenOpenedOnce = false;
    private bool loggedMissingReferences = false;

    void OnValidate()
    {
        if (openDelay < 0f) openDelay = 0f;
    }
    
    void Start()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator != null)
                Debug.Log($"[{gameObject.name}] Auto-assigned Animator");
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                Debug.Log($"[{gameObject.name}] Auto-found Player: {player.name}");
            else
                Debug.LogError($"[{gameObject.name}] Player not found! Tag 'Player' missing or not assigned.");
        }
        
        Debug.Log($"[{gameObject.name}] DoorAnimation initialized:");
        Debug.Log($"  - Player: {(player != null ? player.name : "NULL")}");
        Debug.Log($"  - Interaction Point: {(interactionPoint != null ? interactionPoint.name : "NULL")}");
        Debug.Log($"  - Door Animator: {(doorAnimator != null ? "Assigned" : "NULL")}");
        Debug.Log($"  - Is Locked: {isLocked}");
        Debug.Log($"  - Interaction Distance: {interactionDistance}m");
    }

    void Update()
    {
        if (player == null || interactionPoint == null)
        {
            if (!loggedMissingReferences)
            {
                Debug.LogWarning($"[{gameObject.name}] Cannot update door - Player or InteractionPoint is null!");
                loggedMissingReferences = true;
            }
            return;
        }
        
        if (isLocked)
        {
            if (doorAnimator != null)
                doorAnimator.SetBool("IsOpened", false);
            return;
        }

        float dist = Vector3.Distance(player.transform.position, interactionPoint.position);
        bool inRange = dist <= interactionDistance;

        if (inRange)
            timeInRange += Time.deltaTime;
        else
            timeInRange = 0f;

        bool delayPassed = timeInRange >= openDelay;
        bool allowedToOpen = hasBeenOpenedOnce ? true : delayPassed;

        bool shouldOpen = inRange && allowedToOpen;

        if (shouldOpen && !hasBeenOpenedOnce)
        {
            hasBeenOpenedOnce = true;
            Debug.Log($"[{gameObject.name}] Door opening for first time! Distance: {dist:F2}m, isLocked: {isLocked}");
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("IsOpened", shouldOpen);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Door Animator is null! Cannot play animation.");
        }
    }
    
    public void Unlock()
    {
        isLocked = false;
        loggedMissingReferences = false;
        Debug.Log($"[{gameObject.name}] Door unlocked! isLocked = {isLocked}");
        Debug.Log($"[{gameObject.name}] Player: {(player != null ? player.name : "NULL")}, InteractionPoint: {(interactionPoint != null ? interactionPoint.name : "NULL")}");
    }
    
    public void Lock()
    {
        isLocked = true;
        Debug.Log($"[{gameObject.name}] Door locked! isLocked = {isLocked}");
    }
    
    void OnDrawGizmosSelected()
    {
        if (interactionPoint == null)
            return;
        
        Gizmos.color = isLocked ? Color.red : Color.green;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionDistance);
        
        #if UNITY_EDITOR
        if (player != null)
        {
            float dist = Vector3.Distance(player.transform.position, interactionPoint.position);
            bool inRange = dist <= interactionDistance;
            
            Gizmos.color = inRange ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(interactionPoint.position, player.transform.position);
            
            GUIStyle style = new GUIStyle();
            style.normal.textColor = inRange ? Color.green : Color.red;
            Handles.Label(interactionPoint.position + Vector3.up * 2, 
                $"Distance: {dist:F2}m\n{(inRange ? "IN RANGE" : "OUT OF RANGE")}\nLocked: {isLocked}", style);
        }
        #endif
    }
}
