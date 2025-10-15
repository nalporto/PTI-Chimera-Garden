using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeFrequency = 25f;
    
    private float shakeTimer = 0f;
    private Vector3 originalLocalPosition;
    private bool isShaking = false;

    void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer > 0f)
            {
                float progress = 1f - (shakeTimer / shakeDuration);
                float dampingFactor = 1f - Mathf.Clamp01(progress);
                
                Vector3 shakeOffset = new Vector3(
                    Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f,
                    Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f,
                    0f
                ) * shakeIntensity * dampingFactor;

                transform.localPosition = originalLocalPosition + shakeOffset;
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalLocalPosition;
            }
        }
    }

    public void TriggerShake()
    {
        TriggerShake(shakeIntensity, shakeDuration);
    }

    public void TriggerShake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
        isShaking = true;
        originalLocalPosition = transform.localPosition;
    }

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalLocalPosition;
        shakeTimer = 0f;
    }
}
