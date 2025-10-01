using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator;
    public float interactionDistance = 3f;
    public GameObject player;
    public Transform interactionPoint; // Assign this in the Inspector

    [Header("Delay")]
    [Tooltip("Seconds the player must be inside interactionDistance before the door will open.")]
    public float openDelay = 0.0f;

    private float timeInRange = 0f;

    // Track whether the first delayed open has already occurred
    private bool hasBeenOpenedOnce = false;

    void OnValidate()
    {
        if (openDelay < 0f) openDelay = 0f;
    }

    void Update()
    {
        if (player == null || interactionPoint == null) return;

        float dist = Vector3.Distance(player.transform.position, interactionPoint.position);
        bool inRange = dist <= interactionDistance;

        // accumulate time while player stays in range, reset when they leave
        if (inRange)
            timeInRange += Time.deltaTime;
        else
            timeInRange = 0f;

        // For the first time opening, require the delay. After the first successful open,
        // subsequent opens are immediate when in range.
        bool delayPassed = timeInRange >= openDelay;
        bool allowedToOpen = hasBeenOpenedOnce ? true : delayPassed;

        bool shouldOpen = inRange && allowedToOpen;

        // If this is the first time the door actually opens (after delay), mark it.
        if (shouldOpen && !hasBeenOpenedOnce)
            hasBeenOpenedOnce = true;

        doorAnimator.SetBool("IsOpened", shouldOpen);
    }
}
