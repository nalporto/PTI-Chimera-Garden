using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RoomEnemyManager : MonoBehaviour
{
    [Header("Enemy Tracking")]
    [SerializeField] private Transform enemiesParent;
    [Tooltip("If true, automatically finds all enemies tagged 'Enemy' in this room")]
    [SerializeField] private bool autoDetectEnemies = true;
    
    [Header("Room Detection")]
    [Tooltip("Use a BoxCollider to define room bounds (auto-detect enemies within bounds)")]
    [SerializeField] private BoxCollider roomBounds;
    [Tooltip("If no room bounds, use distance from this manager (0 = infinite)")]
    [SerializeField] private float detectionRadius = 0f;
    
    [Header("Events")]
    public UnityEvent onAllEnemiesKilled;
    
    private int totalEnemies;
    private int enemiesKilled;
    private bool roomCleared = false;
    
    public bool IsRoomCleared => roomCleared;
    public int TotalEnemies => totalEnemies;
    public int EnemiesRemaining => Mathf.Max(0, totalEnemies - enemiesKilled);
    
    void Start()
    {
        CountEnemies();
        InvokeRepeating(nameof(CheckEnemyCount), 0.5f, 0.5f);
    }
    
    private void CountEnemies()
    {
        if (autoDetectEnemies)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            int count = 0;
            
            foreach (GameObject enemy in allEnemies)
            {
                if (IsValidEnemy(enemy) && IsEnemyInRoom(enemy.transform))
                    count++;
            }
            
            totalEnemies = count;
        }
        else if (enemiesParent != null)
        {
            totalEnemies = 0;
            foreach (Transform child in enemiesParent)
            {
                if (child.CompareTag("Enemy") && IsValidEnemy(child.gameObject))
                    totalEnemies++;
            }
        }
        
        Debug.Log($"Room '{gameObject.name}' has {totalEnemies} enemies.");
    }
    
    private bool IsValidEnemy(GameObject obj)
    {
        return obj.GetComponent<HitReceiver>() != null || 
               obj.GetComponent<E_Shooter>() != null ||
               obj.GetComponent<NavMeshAgent>() != null;
    }
    
    private bool IsEnemyInRoom(Transform enemy)
    {
        if (enemiesParent != null)
        {
            Transform current = enemy;
            while (current != null)
            {
                if (current == enemiesParent)
                    return true;
                current = current.parent;
            }
            return false;
        }
        
        if (roomBounds != null)
        {
            return roomBounds.bounds.Contains(enemy.position);
        }
        
        if (detectionRadius > 0f)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            return distance <= detectionRadius;
        }
        
        return false;
    }
    
    private void CheckEnemyCount()
    {
        if (roomCleared) return;
        
        int currentCount = 0;
        
        if (autoDetectEnemies)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in allEnemies)
            {
                if (IsValidEnemy(enemy) && IsEnemyInRoom(enemy.transform))
                    currentCount++;
            }
        }
        else if (enemiesParent != null)
        {
            foreach (Transform child in enemiesParent)
            {
                if (child.CompareTag("Enemy") && IsValidEnemy(child.gameObject))
                    currentCount++;
            }
        }
        
        enemiesKilled = totalEnemies - currentCount;
        
        if (currentCount == 0 && totalEnemies > 0)
        {
            roomCleared = true;
            Debug.Log($"Room '{gameObject.name}' cleared!");
            onAllEnemiesKilled?.Invoke();
            CancelInvoke(nameof(CheckEnemyCount));
        }
    }
    
    public void ResetRoom()
    {
        roomCleared = false;
        enemiesKilled = 0;
        CountEnemies();
        InvokeRepeating(nameof(CheckEnemyCount), 0.5f, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (roomBounds != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.matrix = roomBounds.transform.localToWorldMatrix;
            Gizmos.DrawCube(roomBounds.center, roomBounds.size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(roomBounds.center, roomBounds.size);
        }
        else if (detectionRadius > 0f)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, detectionRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
