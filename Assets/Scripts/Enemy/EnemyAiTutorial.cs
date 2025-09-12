using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAiTutorial : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    public enum AttackType { Melee, Ranged }
    public AttackType attackType = AttackType.Ranged; // Choose in Inspector

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform firePointSecondary;
    public EnemyAnimationController animController;
    [Header("Animator")]
    public Animator animator;

    [Header("Patrol")]
    public LayerMask whatIsGround;
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Detection")]
    public LayerMask whatIsPlayer;
    public float sightRange = 20f;
    public float attackRange = 2f; // Close range

    [Header("Attack")]
    public float timeBetweenAttacks = 2f;
    private float attackCooldown = 0f;
    private bool usePrimaryFirePoint = true;

    [Header("Enemy Stats")]
    public float health = 5f;
    public int damage = 1;
    public int meleeDamage = 10;
    public float meleeRange = 2f;

    public GameObject meleeDamager; // Assign in Inspector (e.g., a child GameObject with a collider)
    private EnemyDamager damagerScript;

    private Renderer[] renderers;
    private Color[] originalColors;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Make sure your player GameObject is tagged 'Player'.");
        }

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else
                originalColors[i] = Color.white;
        }

        if (animController == null)
            animController = GetComponentInChildren<EnemyAnimationController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (meleeDamager != null)
        {
            damagerScript = meleeDamager.GetComponent<EnemyDamager>();
            if (damagerScript != null)
                damagerScript.enemyAI = this; // So the damager can call back to this script
        }
    }

    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log($"{gameObject.name} warped to NavMesh at {hit.position}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} could not find NavMesh nearby!");
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Cooldown timer for attacks
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInSight = distanceToPlayer <= sightRange;
        bool playerInAttack = distanceToPlayer <= attackRange;

        // Set Animator bools for chasing and attacking
        if (animator != null)
        {
            animator.SetBool("IsChasing", currentState == State.Chase);
            animator.SetBool("IsAttacking", currentState == State.Attack && playerInAttack);
        }


        // State transitions and actions
        switch (currentState)
        {
            case State.Patrol:
                if (playerInSight)
                    currentState = State.Chase;
                else
                    Patrol();
                break;

            case State.Chase:
                if (playerInAttack)
                    currentState = State.Attack;
                else if (!playerInSight)
                    currentState = State.Patrol;
                else
                    ChasePlayer();
                break;

            case State.Attack:
                if (!playerInAttack)
                {
                    currentState = State.Chase;
                }
                else
                {
                    AttackPlayer();
                }
                break;
        }
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent.isOnNavMesh)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Fix: Raycast expects the origin as a Vector3, not a float
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(player.position);

        FacePlayer();
    }

    private void AttackPlayer()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(transform.position); // Stop moving

        FacePlayer();

        if (attackCooldown <= 0f)
        {
            if (animController != null)
                animController.PlayAttack();

            if (attackType == AttackType.Ranged)
            {
                // Fire projectile
                Transform chosenFirePoint = firePoint;
                if (firePointSecondary != null)
                {
                    chosenFirePoint = usePrimaryFirePoint ? firePoint : firePointSecondary;
                    usePrimaryFirePoint = !usePrimaryFirePoint;
                }

                Vector3 targetPos = player.position + Vector3.up * 1.0f;
                Vector3 dir = (targetPos - chosenFirePoint.position).normalized;

                Instantiate(projectilePrefab, chosenFirePoint.position, Quaternion.LookRotation(dir));
            }
            else if (attackType == AttackType.Melee)
            {
                // Enable the damager for a short time (e.g., during the attack animation)
                if (damagerScript != null)
                    damagerScript.EnableDamagerForDuration(0.3f); // 0.3s or match your animation
            }

            attackCooldown = timeBetweenAttacks;
        }
    }

    private void FacePlayer()
    {
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy hit! HP remaining: {health}");

        StartCoroutine(HitFlash());

        if (health <= 0)
            DestroyEnemy();
    }

    private IEnumerator HitFlash()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = Color.white;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}