using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    private bool onFire = false;
    private float fireDuration = 3f;
    private float fireTick = 0.5f;
    private float fireDamage = 1f;

    [Header("Fire Effect")]
    [SerializeField] private ParticleSystem fireEffectPrefab;
    private ParticleSystem activeFireEffect;

    public void ApplyFireStatus()
    {
        if (!onFire)
            StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        onFire = true;

        // Spawn fire effect at center, scale to enemy size + 0.3
        if (fireEffectPrefab != null && activeFireEffect == null)
        {
            activeFireEffect = Instantiate(fireEffectPrefab, GetCenter(), Quaternion.identity, transform);

            // Calculate scale: enemy bounds + 0.3
            Vector3 scale = GetComponent<Renderer>() != null
                ? GetComponent<Renderer>().bounds.size
                : Vector3.one;
            scale += Vector3.one * 0.3f;
            activeFireEffect.transform.localScale = scale;
        }

        float elapsed = 0f;
        while (elapsed < fireDuration)
        {
            EnemyAiTutorial ai = GetComponent<EnemyAiTutorial>();
            if (ai != null)
                ai.TakeDamage(fireDamage);

            elapsed += fireTick;
            yield return new WaitForSeconds(fireTick);
        }
        onFire = false;

        // Stop and destroy fire effect
        if (activeFireEffect != null)
        {
            activeFireEffect.Stop();
            Destroy(activeFireEffect.gameObject, 1f);
            activeFireEffect = null;
        }
    }

    private Vector3 GetCenter()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            return rend.bounds.center;
        return transform.position;
    }
}