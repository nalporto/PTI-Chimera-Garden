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
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        // else: do nothing (projectile keeps going)
    }
}