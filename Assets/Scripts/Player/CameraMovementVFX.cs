using UnityEngine;

public class CameraMovementLines : MonoBehaviour
{
    [SerializeField] private ParticleSystem movementLines;
    [SerializeField] private Transform player;
    private Vector3 lastPosition;

    void Start()
    {
        if (player == null)
            player = transform; // fallback
        lastPosition = player.position;
    }

    void Update()
    {
        float speed = (player.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = player.position;

        if (speed > 1f) // Only show lines if moving fast enough
        {
            if (!movementLines.isEmitting)
                movementLines.Play();
        }
        else
        {
            if (movementLines.isEmitting)
                movementLines.Stop();
        }
    }
}
