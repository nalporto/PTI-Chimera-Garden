using UnityEngine;

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

    void OnValidate()
    {
        if (openDelay < 0f) openDelay = 0f;
    }

    void Update()
    {
        if (player == null || interactionPoint == null) return;
        
        if (isLocked)
        {
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
            Debug.Log($"[{gameObject.name}] Door opening for first time! Distance: {dist:F2}m");
        }

        doorAnimator.SetBool("IsOpened", shouldOpen);
    }
    
    public void Unlock()
    {
        isLocked = false;
        Debug.Log($"[{gameObject.name}] Door unlocked! isLocked = {isLocked}");
    }
    
    public void Lock()
    {
        isLocked = true;
        Debug.Log($"[{gameObject.name}] Door locked! isLocked = {isLocked}");
    }
}
