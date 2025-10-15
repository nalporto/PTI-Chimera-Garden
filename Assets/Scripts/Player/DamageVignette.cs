using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image vignetteImage;
    
    [Header("Damage Flash Settings")]
    [SerializeField] private Color damageColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    [Header("Low Health Vignette")]
    [SerializeField] private bool enableLowHealthVignette = true;
    [SerializeField] private float lowHealthThreshold = 0.3f;
    [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f, 0.15f);
    [SerializeField] private float pulseDuration = 1.5f;
    [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 1f);

    private Coroutine flashCoroutine;
    private Coroutine pulseCoroutine;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (vignetteImage == null)
        {
            vignetteImage = GetComponent<Image>();
        }

        if (vignetteImage != null)
        {
            vignetteImage.color = Color.clear;
        }
        else
        {
            Debug.LogError("DamageVignette: No Image component found! Please assign a UI Image.");
        }

        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (enableLowHealthVignette && playerHealth != null && vignetteImage != null)
        {
            float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
            
            if (healthPercent <= lowHealthThreshold && healthPercent > 0f)
            {
                if (pulseCoroutine == null)
                {
                    pulseCoroutine = StartCoroutine(LowHealthPulse());
                }
            }
            else
            {
                if (pulseCoroutine != null)
                {
                    StopCoroutine(pulseCoroutine);
                    pulseCoroutine = null;
                }
            }
        }
    }

    public void TriggerDamageFlash()
    {
        if (vignetteImage == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        
        flashCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            float curveValue = flashCurve.Evaluate(t);
            
            Color targetColor = damageColor;
            targetColor.a = damageColor.a * curveValue;
            
            vignetteImage.color = targetColor;
            
            yield return null;
        }

        vignetteImage.color = Color.clear;
        flashCoroutine = null;
    }

    private IEnumerator LowHealthPulse()
    {
        while (true)
        {
            float elapsed = 0f;

            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pulseDuration;
                float curveValue = pulseCurve.Evaluate(t);
                
                Color targetColor = lowHealthColor;
                targetColor.a = lowHealthColor.a * curveValue;
                
                if (flashCoroutine == null)
                {
                    vignetteImage.color = targetColor;
                }
                
                yield return null;
            }
        }
    }

    public void ClearVignette()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (vignetteImage != null)
        {
            vignetteImage.color = Color.clear;
        }
    }
}
