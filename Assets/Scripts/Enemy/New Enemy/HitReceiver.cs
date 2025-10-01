using UnityEngine;

public class HitReceiver : MonoBehaviour
{
    [Tooltip("Health for this enemy body. Damage from Shooter will reduce this.")]
    public float health = 20f;

    // Called by Shooter when raycast hits this enemy (or its children)
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {health}");

        if (health <= 0f)
        {
            Die();
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
}
