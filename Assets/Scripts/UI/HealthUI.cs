using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Visual Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float mediumHealthThreshold = 50f;
    [SerializeField] private float lowHealthThreshold = 25f;
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionSpeed = 5f;

    private Image healthBarImage;
    private Vector2 originalBarSize;
    private float targetFillAmount;
    private float currentFillAmount;

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        }

        if (healthBarFill != null)
        {
            healthBarImage = healthBarFill.GetComponent<Image>();
            originalBarSize = healthBarFill.sizeDelta;
            currentFillAmount = 1f;
            targetFillAmount = 1f;
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            UpdateHealthDisplay();
        }
    }

    private void UpdateHealthDisplay()
    {
        int current = playerHealth.CurrentHealth;
        int max = playerHealth.MaxHealth;
        float healthPercentage = (float)current / max;

        targetFillAmount = healthPercentage;

        if (smoothTransition)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
        }
        else
        {
            currentFillAmount = targetFillAmount;
        }

        if (healthText != null)
        {
            int percentage = Mathf.RoundToInt(healthPercentage * 100f);
            healthText.text = $"{percentage}%";
        }

        if (healthBarFill != null)
        {
            Vector2 newSize = originalBarSize;
            newSize.x = originalBarSize.x * currentFillAmount;
            healthBarFill.sizeDelta = newSize;

            if (healthBarImage != null)
            {
                Color targetColor;
                if (current <= lowHealthThreshold)
                    targetColor = lowHealthColor;
                else if (current <= mediumHealthThreshold)
                    targetColor = mediumHealthColor;
                else
                    targetColor = fullHealthColor;

                healthBarImage.color = targetColor;
            }
        }
    }
}