using UnityEngine;
using TMPro;

public class AmmoWarning : MonoBehaviour
{
    [SerializeField] private WeaponSwitching weaponSwitching; // Reference to WeaponSwitching
    [SerializeField] private TextMeshProUGUI warningText; // Reference to the TMP text
    [SerializeField] private Color outOfAmmoColor = Color.red;

    void Start()
    {
        if (warningText != null)
            warningText.enabled = false;
    }

    void Update()
    {
        if (weaponSwitching != null && warningText != null)
        {
            Transform weaponTransform = weaponSwitching.transform.GetChild(weaponSwitching.selectedWeapon);
            Shooter shooter = weaponTransform.GetComponent<Shooter>();

            if (shooter != null && shooter.CurrentAmmo <= 0 && !shooter.IsReloading)
            {
                warningText.enabled = true;
                warningText.text = "OUT OF AMMO";
                warningText.color = outOfAmmoColor;
            }
            else
            {
                warningText.enabled = false;
            }
        }
    }
}
