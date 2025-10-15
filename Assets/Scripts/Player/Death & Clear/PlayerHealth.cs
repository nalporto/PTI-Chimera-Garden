using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Damage Feedback")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private DamageVignette damageVignette;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioSource audioSource;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        
        if (cameraShake == null)
            cameraShake = FindObjectOfType<CameraShake>();
        
        if (damageVignette == null)
            damageVignette = FindObjectOfType<DamageVignette>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"Player hit! HP remaining: {currentHealth}");
        
        if (cameraShake != null)
            cameraShake.TriggerShake();
        
        if (damageVignette != null)
            damageVignette.TriggerDamageFlash();
        
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);
        
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
        
        if (damageVignette != null)
            damageVignette.ClearVignette();
        
        if (DeathScreen.Instance != null)
        {
            DeathScreen.Instance.RestartGame();
        }
    }
}
