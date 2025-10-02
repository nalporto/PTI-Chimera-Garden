using UnityEngine;

public class Swinger : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;         // Range to hit the player
    public float attackInterval = 1.2f;    // Time between attacks
    public int damageAmount = 10;          // Damage dealt per hit

    [Header("References")]
    public Animator animator;              // Assign your Animator in Inspector
    public RangeDetector rangeDetector;    // Assign your RangeDetector in Inspector

    private float attackTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (rangeDetector == null || animator == null)
            return;

        GameObject target = rangeDetector.DetectedTarget;
        if (target == null)
        {
            attackTimer = 0f;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= attackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                Attack(target);
                attackTimer = 0f;
            }
        }
        else
        {
            attackTimer = 0f;
        }
    }

    void Attack(GameObject target)
    {
        // Play attack animation
        animator.SetTrigger("Attack");

        // Damage logic (requires the player to have a script with a TakeDamage(int amount) method)
        var health = target.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
    }
}
