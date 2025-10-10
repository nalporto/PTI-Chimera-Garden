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
        // Auto-assign CoreMagic as targetA if not manually assigned
        if (targetA == null)
        {
            Transform coreMagic = transform.Find("CoreMagic");
            if (coreMagic != null)
            {
                targetA = coreMagic;
                Debug.Log($"Auto-assigned CoreMagic as targetA for {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"CoreMagic child not found in {gameObject.name}. Please assign targetA manually.");
            }
        }
        
        // Only initialize positions if targets are assigned
        if (targetA != null)
            initialPositionA = targetA.localPosition;
        if (targetB != null)
            initialPositionB = targetB.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Move up and down on Z axis
        float zOffset = Mathf.Sin(Time.time * moveFrequency) * moveAmplitude;

        // Apply movement and rotation to targetA if assigned
        if (targetA != null)
        {
            targetA.localPosition = initialPositionA + new Vector3(0f, 0f, zOffset);
            targetA.Rotate(rotationSpeed * Time.deltaTime);
        }
        
        // Apply movement and rotation to targetB if assigned
        if (targetB != null)
        {
            targetB.localPosition = initialPositionB + new Vector3(0f, 0f, zOffset);
            targetB.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}
