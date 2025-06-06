using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Shooter playerWeapon;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.red;
    [SerializeField] private int lowAmmoThreshold = 3;

    void Update()
    {
        if (playerWeapon != null && ammoText != null)
        {
            ammoText.text = $"{playerWeapon.CurrentAmmo} / {playerWeapon.MagSize}";

            // Change color if low on ammo
            if (playerWeapon.CurrentAmmo <= lowAmmoThreshold)
                ammoText.color = lowAmmoColor;
            else
                ammoText.color = normalColor;
        }
    }
}