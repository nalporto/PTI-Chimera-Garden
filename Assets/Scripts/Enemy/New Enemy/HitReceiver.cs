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
    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {health}");
        
        StartCoroutine(HitFlash());

        if (health <= 0f)
        {
            Die();
        }
    }
    
    private IEnumerator HitFlash()
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                propertyBlock.SetColor("_BaseColor", flashColor);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
        
        yield return new WaitForSeconds(flashDuration);
        
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.SetPropertyBlock(null);
            }
        }
    }

    private void Die()
    {
        GameObject enemyRoot = null;

        var ai = GetComponentInParent<EnemyAiTutorial>();
        if (ai != null)
            enemyRoot = ai.gameObject;
        else
        {
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
}
