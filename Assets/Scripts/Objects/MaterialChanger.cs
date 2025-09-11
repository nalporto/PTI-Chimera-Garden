using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [Header("References")]
    public GameObject targetObject; // The object whose material will change
    public Material newMaterial;    // The material to switch to
    public GameObject player;       // Reference to the player

    [Header("Settings")]
    public float changeDistance = 3f; // Distance at which material changes

    private Material originalMaterial;
    private Renderer targetRenderer;

    void Start()
    {
        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                originalMaterial = targetRenderer.material;
            }
        }
    }

    void Update()
    {
        if (player == null || targetRenderer == null || newMaterial == null)
            return;

        float dist = Vector3.Distance(player.transform.position, targetObject.transform.position);

        if (dist <= changeDistance)
        {
            if (targetRenderer.material != newMaterial)
                targetRenderer.material = newMaterial;
        }
        else
        {
            if (targetRenderer.material != originalMaterial)
                targetRenderer.material = originalMaterial;
        }
    }
}