using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform firePointSecondary;

    [Header("Patrol")]
    public LayerMask whatIsGround;
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Detection")]
    public LayerMask whatIsPlayer;
    public float sightRange = 20f, attackRange = 10f;
    private bool playerInSightRange, playerInAttackRange;

    [Header("Attack")]
    public float timeBetweenAttacks = 2f;
    private bool alreadyAttacked = false;
    private bool usePrimaryFirePoint = true;

    [Header("Enemy Stats")]
    public float health = 5f;
    public int damage = 1;

    private Renderer[] renderers;
    private Color[] originalColors;
    private EnemyAnimationController animController;

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

        animController = GetComponentInChildren<EnemyAnimationController>();
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
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Check if the player is visible (not obstructed by walls)
        if (playerInSightRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);


        }

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent.isOnNavMesh)
            agent.SetDestination(walkPoint);
        else
            Debug.LogWarning($"{gameObject.name} is not on the NavMesh!");

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if (player != null)
        {
            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (!alreadyAttacked && player != null)
        {
            // Trigger attack animation
            if (animController != null)
                animController.PlayAttack();

            Transform chosenFirePoint = firePoint;
            if (firePointSecondary != null)
            {
                chosenFirePoint = usePrimaryFirePoint ? firePoint : firePointSecondary;
                usePrimaryFirePoint = !usePrimaryFirePoint;
            }

            Vector3 targetPos = player.position + Vector3.up * 1.0f;
            Vector3 dir = (targetPos - chosenFirePoint.position).normalized;

            Instantiate(projectilePrefab, chosenFirePoint.position, Quaternion.LookRotation(dir));

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
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