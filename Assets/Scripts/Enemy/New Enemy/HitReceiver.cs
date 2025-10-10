using UnityEngine;
using System.Collections;

public class HitReceiver : MonoBehaviour
{
    [Tooltip("Health for this enemy body. Damage from Shooter will reduce this.")]
    public float health = 20f;
    
    [Header("Hit Flash Effect")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.white;
    
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private Material flashMaterial;

    void Start()
    {
        // Get all renderers in this enemy (including children)
        renderers = GetComponentsInChildren<Renderer>();
        
        // Store original materials for each renderer
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
        
        // Create flash material
        CreateFlashMaterial();
    }
    
    private void CreateFlashMaterial()
    {
        // Create a simple unlit material for the flash effect
        flashMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        flashMaterial.SetColor("_BaseColor", flashColor);
    }

    // Called by Shooter when raycast hits this enemy (or its children)
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {health}");
        
        // Trigger hit flash effect
        StartCoroutine(HitFlash());

        if (health <= 0f)
        {
            Die();
        }
    }
    
    private IEnumerator HitFlash()
    {
        // Switch all materials to flash material
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                Material[] flashMaterials = new Material[renderers[i].materials.Length];
                for (int j = 0; j < flashMaterials.Length; j++)
                {
                    flashMaterials[j] = flashMaterial;
                }
                renderers[i].materials = flashMaterials;
            }
        }
        
        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);
        
        // Restore original materials
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && originalMaterials[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
    }

    private void Die()
    {
        // Destroy only the enemy instance, not the whole root parent container.
        // Prefer the GameObject that has the EnemyAiTutorial (if present),
        // otherwise use the closest parent with tag "Enemy", otherwise this gameObject.
        GameObject enemyRoot = null;

        var ai = GetComponentInParent<EnemyAiTutorial>();
        if (ai != null)
            enemyRoot = ai.gameObject;
        else
        {
            // try find nearest parent tagged as "Enemy"
            Transform t = transform;
            while (t != null)
            {
                if (t.CompareTag("Enemy"))
                {
                    enemyRoot = t.gameObject;
                    break;
                }
                t = t.parent;
            }
        }

        if (enemyRoot == null)
            enemyRoot = gameObject;

        Debug.Log($"{enemyRoot.name} died.");
        Destroy(enemyRoot);
    }
    
    void OnDestroy()
    {
        // Clean up flash material
        if (flashMaterial != null)
        {
            Destroy(flashMaterial);
        }
    }
}
