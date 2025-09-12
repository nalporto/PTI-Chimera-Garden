using UnityEngine;
using System.Collections;

public class EnemyDamager : MonoBehaviour
{
    public EnemyAiTutorial enemyAI;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false; // Start disabled
    }

    public void EnableDamagerForDuration(float duration)
    {
        if (col != null)
            StartCoroutine(DamageRoutine(duration));
    }

    private IEnumerator DamageRoutine(float duration)
    {
        col.enabled = true;
        yield return new WaitForSeconds(duration);
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyAI == null || enemyAI.attackType != EnemyAiTutorial.AttackType.Melee)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyAI.meleeDamage);
            }
        }
    }
}
