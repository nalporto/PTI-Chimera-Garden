using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator component
    public float interactionDistance = 3f; // Distance within which the player can interact
    public GameObject player; // Reference to the player object (assignable in the Inspector)

    void Update()
    {
        if (player != null && Vector3.Distance(player.transform.position, transform.position) <= interactionDistance)
        {
            doorAnimator.SetBool("IsOpened", true); // Keep the door open while the player is near
        }
        else
        {
            doorAnimator.SetBool("IsOpened", false); // Close the door when the player moves away
        }
    }
}
