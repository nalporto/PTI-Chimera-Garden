using UnityEngine;

public class CameraMovementLines : MonoBehaviour
{
    [SerializeField] private ParticleSystem movementLines;
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distanceInFront = 1.5f;

    private Vector3 lastPosition;

    void Start()
    {
        if (player == null)
            player = transform;
        lastPosition = player.position;
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float speed = (player.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = player.position;

        if (movementLines != null && cameraTransform != null)
        {
            movementLines.transform.position = cameraTransform.position + cameraTransform.forward * distanceInFront;
            movementLines.transform.rotation = cameraTransform.rotation;

            if (speed > 25f)
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
}
