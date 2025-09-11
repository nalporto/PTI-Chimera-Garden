using UnityEngine;
using UnityEngine.AI;

public class EnemyBehauvior : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4f;
    public float timeToRotate = 1f;
    public float speedWalk = 6f;
    public float speedRun = 9f;

    public float viewRadius = 10f;
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeResolveIterations = 4;
    public float edgeDistance = 0.5f;
    public Transform[] waypoints;
    public Transform player; // Assign the player transform in the inspector
    int m_CurrentWaypointIndex;

    Vector3 m_PlayerLastKnownPosition = Vector3.zero;
    Vector3 m_PlayerPosition = Vector3.zero;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrolling;
    bool m_IsChasing;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrolling = true;
        m_IsChasing = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        EnviromentView();
        if (!m_IsPatrolling)
        {
            Chasing();
        }
        else
        {
            Patrolling();
        }
    }

    private void Chasing()
    {
        m_PlayerNear = false;
        m_PlayerLastKnownPosition = Vector3.zero;

        if (!m_IsChasing)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);
            m_IsChasing = true;
        }
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0)
            {
                m_IsPatrolling = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
                m_IsChasing = false;
            }
            else
            {
                if (player != null && Vector3.Distance(transform.position, player.position) <= viewRadius)
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patrolling()
    {
        if (m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(m_PlayerLastKnownPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            m_PlayerLastKnownPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
    }

    public void NextPoint()
    {
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
    }

    void LookingPlayer(Vector3 playerPosition)
    {
        navMeshAgent.SetDestination(playerPosition);
        if (Vector3.Distance(transform.position, playerPosition) <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        m_PlayerInRange = false;
        for (int i = 0; i < playerInRange.Length; i++)
        {
            Vector3 playerPos = playerInRange[i].transform.position;
            Vector3 dirToPlayer = (playerPos - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerPos);
                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrolling = false;
                    m_PlayerPosition = playerPos;
                }
            }
        }
    }
}