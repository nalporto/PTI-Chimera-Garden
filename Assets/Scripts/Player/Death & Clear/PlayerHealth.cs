using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"Player hit! HP remaining: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player died!");
        
        if (DeathScreen.Instance != null)
        {
            DeathScreen.Instance.TriggerDeathScreen();
        }
    }
    
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        if (DeathScreen.Instance != null)
        {
            DeathScreen.Instance.RestartGame();
        }
    }
}
