using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Example method for taking damage
    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"Player hit! HP remaining: {currentHealth}");
        // Add death logic if needed
    }

    // Example method for healing
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
