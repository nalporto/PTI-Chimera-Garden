using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 45f;
    public float lifetime = 5f;
    private Rigidbody rb;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; 
        }
        
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"));
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 nextPosition = transform.position + transform.forward * speed * Time.deltaTime;
        
        int layerMask = ~LayerMask.GetMask("Enemy", "Weapons");
        
        if (Physics.Linecast(transform.position, nextPosition, out hit, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Enemy projectile hit player for {damage} damage");
                }
            }
            else if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Wall"))
            {
                Debug.Log($"Enemy projectile hit {hit.collider.tag}: {hit.collider.name}");
            }
            
            Destroy(gameObject);
            return;
        }
        
        transform.position = nextPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Enemy projectile (trigger) hit player for {damage} damage");
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Debug.Log($"Enemy projectile (trigger) hit {other.tag}: {other.name}");
            Destroy(gameObject);
        }
    }
}