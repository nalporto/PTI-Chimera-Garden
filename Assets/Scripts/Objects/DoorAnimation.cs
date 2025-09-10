using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator;
    public float interactionDistance = 3f;
    public GameObject player;
    public Transform interactionPoint; // Assign this in the Inspector

    void Update()
    {
        if (player == null || interactionPoint == null) return;

        float dist = Vector3.Distance(player.transform.position, interactionPoint.position);
        bool inRange = dist <= interactionDistance;
        doorAnimator.SetBool("IsOpened", inRange);

    }
}
