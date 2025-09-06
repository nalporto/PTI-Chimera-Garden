using UnityEngine;

public class MagicController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem magicEffect; // Assign a child or attached particle system

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDrainRate = 25f; // per second

    [Header("Time Settings")]
    [SerializeField] private float slowTimeScale = 0.3f; // How slow time gets

    private float currentEnergy;
    private bool isSlowingTime = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnergy = maxEnergy;
        if (magicEffect != null)
            magicEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E) && currentEnergy > 0f)
        {
            if (!isSlowingTime)
            {
                isSlowingTime = true;
                Time.timeScale = slowTimeScale;
                if (magicEffect != null && !magicEffect.isPlaying)
                    magicEffect.Play();
            }

            currentEnergy -= energyDrainRate * Time.unscaledDeltaTime;
            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                StopSlowTime();
            }
        }
        else
        {
            if (isSlowingTime)
            {
                StopSlowTime();
            }
            // Recover energy at 10 per second when not slowing time
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += 10f * Time.unscaledDeltaTime;
                if (currentEnergy > maxEnergy)
                    currentEnergy = maxEnergy;
            }
        }
    }

    void StopSlowTime()
    {
        isSlowingTime = false;
        Time.timeScale = 1f;
        if (magicEffect != null && magicEffect.isPlaying)
            magicEffect.Stop();
    }

    public float GetCurrentEnergy() => currentEnergy;
    public float GetMaxEnergy() => maxEnergy;

    // Optional: Call this to refill energy
    public void RefillEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
    }
}
