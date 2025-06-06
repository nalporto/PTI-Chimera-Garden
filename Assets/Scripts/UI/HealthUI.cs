using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowColor = Color.red;
    [SerializeField] private int lowHealthThreshold = 25;

    void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            int current = playerHealth.CurrentHealth;
            int max = playerHealth.MaxHealth;
            healthText.text = $"Health: {current}";

            // Change color based on health
            healthText.color = current <= lowHealthThreshold ? lowColor : normalColor;
        }
    }
}