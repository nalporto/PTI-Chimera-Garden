using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private RectTransform energyBar; // Changed to RectTransform
    [SerializeField] private MagicController magicController;
    
    [Header("Visual Settings")]
    [SerializeField] private Color fullEnergyColor = Color.cyan;
    [SerializeField] private Color lowEnergyColor = Color.red;
    [SerializeField] private Color criticalEnergyColor = Color.red;
    [SerializeField] private float lowEnergyThreshold = 25f;
    [SerializeField] private float criticalEnergyThreshold = 10f;
    
    private Image energyBarImage;
    private Vector2 originalBarSize;
    
    void Start()
    {
        if (magicController == null)
        {
            magicController = Object.FindFirstObjectByType<MagicController>();
        }
        
        if (energyBar != null)
        {
            energyBarImage = energyBar.GetComponent<Image>();
            originalBarSize = energyBar.sizeDelta;
        }
    }
    
    void Update()
    {
        if (magicController != null)
        {
            UpdateEnergyDisplay();
        }
    }
    
    private void UpdateEnergyDisplay()
    {
        float currentEnergy = magicController.GetCurrentEnergy();
        float maxEnergy = magicController.GetMaxEnergy();
        float energyPercentage = currentEnergy / maxEnergy;
        
        // Update text
        if (energyText != null)
        {
            energyText.text = $"Energy: {Mathf.RoundToInt(currentEnergy)}";
        }
        
        // Update energy bar using width scaling
        if (energyBar != null)
        {
            Vector2 newSize = originalBarSize;
            newSize.x = originalBarSize.x * energyPercentage;
            energyBar.sizeDelta = newSize;
            
            // Change color based on energy level
            if (energyBarImage != null)
            {
                Color targetColor;
                if (currentEnergy <= criticalEnergyThreshold)
                    targetColor = criticalEnergyColor;
                else if (currentEnergy <= lowEnergyThreshold)
                    targetColor = lowEnergyColor;
                else
                    targetColor = fullEnergyColor;
                
                energyBarImage.color = targetColor;
            }
        }
    }
}
