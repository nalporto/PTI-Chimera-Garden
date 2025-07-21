using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private WeaponSwitching weaponSwitching; // Reference to WeaponSwitching
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.red;
    [SerializeField] private int lowAmmoThreshold = 3;

    void Update()
    {
        if (weaponSwitching != null && ammoText != null)
        {
            // Get the currently selected weapon's Shooter component
            Transform weaponTransform = weaponSwitching.transform.GetChild(weaponSwitching.selectedWeapon);
            Shooter currentShooter = weaponTransform.GetComponent<Shooter>();

            if (currentShooter != null)
            {
                ammoText.text = $"{currentShooter.CurrentAmmo} / {currentShooter.MagSize}";

                // Change color if low on ammo
                if (currentShooter.CurrentAmmo <= lowAmmoThreshold)
                    ammoText.color = lowAmmoColor;
                else
                    ammoText.color = normalColor;
            }
            else
            {
                ammoText.text = "-- / --";
                ammoText.color = normalColor;
            }
        }
    }
}