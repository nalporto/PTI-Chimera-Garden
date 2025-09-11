using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Light targetLight; // Assign the Point Light here (can be child or not)
    public GameObject targetObject; // The object whose material will change
    public Material newMaterial;    // The material to switch to

    [Header("Settings")]
    public float changeDistance = 3f;
    public Color closeColor = Color.green;
    public Color farColor = Color.white;

    private Color originalColor;
    private Material originalMaterial;
    private Renderer targetRenderer;

    void Start()
    {
        if (targetLight != null)
        {
            originalColor = targetLight.color;
        }

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
        if (player == null)
            return;

        float dist = Vector3.Distance(player.transform.position, transform.position);

        // Light color change
        if (targetLight != null)
        {
            if (dist <= changeDistance)
                targetLight.color = closeColor;
            else
                targetLight.color = farColor;
        }

        // Material change
        if (targetRenderer != null && newMaterial != null && originalMaterial != null)
        {
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
}