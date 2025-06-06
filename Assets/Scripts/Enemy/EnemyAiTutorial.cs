using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Add this for coroutine support

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;

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

    [Header("Enemy Stats")]
    public float health = 5f;
    public int damage = 1;

    private Renderer[] renderers; // Store all renderers for color change
    private Color[] originalColors;

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

        // Cache all renderers and their original colors
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else
                originalColors[i] = Color.white;
        }
    }

    private void Update()
    {
        // Detection
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

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

            // Always face the player
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
            // Always face the player
            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (!alreadyAttacked && player != null)
        {
            // Calculate direction to player
            Vector3 targetPos = player.position + Vector3.up * 1.0f; // Aim at player's chest/head
            Vector3 dir = (targetPos - firePoint.position).normalized;

            // Instantiate at firePoint
            Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Enemy hit! HP remaining: {health}");

        StartCoroutine(HitFlash());

        if (health <= 0)
            DestroyEnemy(); // Instantly destroy enemy
    }

    private IEnumerator HitFlash()
    {
        // Set all materials to white
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = Color.white;
        }
        yield return new WaitForSeconds(0.1f);
        // Restore original colors
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
