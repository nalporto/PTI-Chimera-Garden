using UnityEngine;
using UnityEngine.AI;

public class E_Shooter : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public RangeDetector rangeDetector;
    public Animator animator;

    [Header("Settings")]
    public float walkSpeed = 7f; // Add this line for walk speed
    public float shootInterval = 1f;
    public float shootingRadius = 10f;      // Distance at which enemy stops and shoots

    [Header("Aiming")]
    [Tooltip("If > 0 used to compute a lead on a moving target. Set to 0 to aim directly at the target.")]
    public float projectileSpeed = 0f;
    public bool useLead = true;
    public Vector3 aimOffset = Vector3.up * 1.0f; // offset to aim at (head/torso)

    private float shootTimer = 0f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = walkSpeed; // Set walk speed here
        }
    }

    void Update()
    {
        if (projectilePrefab == null || firePoint == null || rangeDetector == null || animator == null)
            return;

        GameObject target = rangeDetector.DetectedTarget;

        // If no target detected, enemy is idle
        if (target == null)
        {
            if (agent != null && agent.isActiveAndEnabled)
                agent.isStopped = false; // Resume moving

            animator.SetBool("IsIdle", true);
            animator.SetBool("IsChasing", false);
            shootTimer = 0f;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // Player detected, not idle anymore
        animator.SetBool("IsIdle", false);

        // Chasing if player is in detection radius but not in shooting radius
        if (distanceToTarget > shootingRadius)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.speed = walkSpeed; // <-- Force speed here
            }
            animator.SetBool("IsChasing", true);
        }
        // In shooting radius, stop and shoot
        else
        {
            if (agent != null && agent.isActiveAndEnabled)
                agent.isStopped = true; // Stop moving

            animator.SetBool("IsChasing", false);

            // Optionally, look at the target
            Vector3 lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookPos);

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                ShootAtTarget(target);
                shootTimer = 0f;
            }
        }
    }

    void ShootAtTarget(GameObject target)
    {
        if (projectilePrefab == null || firePoint == null || target == null)
            return;

        // Compute aim position (target position + optional offset)
        Vector3 aimPos = target.transform.position + aimOffset;

        // Determine rotation / direction to fire
        Quaternion fireRotation;
        if (useLead && projectileSpeed > 0f)
        {
            // try to get target velocity (if it has a Rigidbody)
            Vector3 targetVel = Vector3.zero;
            var rb = target.GetComponent<Rigidbody>();
            if (rb != null) targetVel = rb.linearVelocity;

            Vector3 dir = FirstOrderIntercept(firePoint.position, Vector3.zero, projectileSpeed, aimPos, targetVel);
            fireRotation = Quaternion.LookRotation(dir);
        }
        else
        {
            fireRotation = Quaternion.LookRotation((aimPos - firePoint.position).normalized);
        }

        // Instantiate and orient projectile so it travels toward computed direction
        var proj = Instantiate(projectilePrefab, firePoint.position, fireRotation);

        // Trigger attack animation
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    // First-order intercept: returns the aim direction (normalized) accounting for target velocity
    Vector3 FirstOrderIntercept(Vector3 shooterPos, Vector3 shooterVel, float shotSpeed, Vector3 targetPos, Vector3 targetVel)
    {
        Vector3 dirToTarget = targetPos - shooterPos;
        Vector3 relVel = targetVel - shooterVel;

        float a = Vector3.Dot(relVel, relVel) - shotSpeed * shotSpeed;
        float b = 2f * Vector3.Dot(relVel, dirToTarget);
        float c = Vector3.Dot(dirToTarget, dirToTarget);

        float disc = b * b - 4f * a * c;
        if (disc < 0f || Mathf.Abs(a) < 0.0001f)
        {
            // no good solution -> aim at current position
            return dirToTarget.normalized;
        }

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        if (t <= 0f)
            return dirToTarget.normalized;

        Vector3 aimPoint = dirToTarget + relVel * t;
        return aimPoint.normalized;
    }
}
