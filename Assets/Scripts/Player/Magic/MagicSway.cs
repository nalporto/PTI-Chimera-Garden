using UnityEngine;

public class Magicswaying : MonoBehaviour
{
    [SerializeField] private Transform targetA; // First object to move/rotate
    [SerializeField] private Transform targetB; // Second object to move/rotate
    [SerializeField] private float moveAmplitude = 0.2f;
    [SerializeField] private float moveFrequency = 1.5f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(30f, 45f, 60f);

    private Vector3 initialPositionA;
    private Vector3 initialPositionB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetA == null) targetA = transform;
        if (targetB == null) targetB = transform;
        initialPositionA = targetA.localPosition;
        initialPositionB = targetB.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Move up and down on Z axis
        float zOffset = Mathf.Sin(Time.time * moveFrequency) * moveAmplitude;

        targetA.localPosition = initialPositionA + new Vector3(0f, 0f, zOffset);
        targetB.localPosition = initialPositionB + new Vector3(0f, 0f, zOffset);

        // Rotate on all axes
        targetA.Rotate(rotationSpeed * Time.deltaTime);
        targetB.Rotate(rotationSpeed * Time.deltaTime);
    }
}
