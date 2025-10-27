using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponSwitching weaponSwitching;
    [SerializeField] private Image weaponIconImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Transform bulletIconsContainer;
    [SerializeField] private GameObject bulletIconPrefab;
    
    [Header("Weapon Data")]
    [SerializeField] private WeaponDisplayData[] weaponData;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalAmmoColor = new Color(1f, 0.65f, 0.2f);
    [SerializeField] private Color lowAmmoColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color emptyBulletColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private int lowAmmoThreshold = 3;
    [SerializeField] private int maxBulletIcons = 30;
    
    [Header("Animation")]
    [SerializeField] private float weaponSwitchAnimSpeed = 5f;
    [SerializeField] private float ammoUpdateAnimSpeed = 10f;
    
    private Image[] bulletIcons;
    private int lastAmmoCount = -1;
    private int lastWeaponIndex = -1;
    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        InitializeBulletIcons();
        
        if (weaponSwitching != null)
            lastWeaponIndex = weaponSwitching.selectedWeapon;
        
        UpdateWeaponDisplay();
    }
    
    void Update()
    {
        UpdateWeaponDisplay();
    }
    
    private void InitializeBulletIcons()
    {
        if (bulletIconsContainer == null || bulletIconPrefab == null)
            return;
        
        foreach (Transform child in bulletIconsContainer)
            Destroy(child.gameObject);
        
        bulletIcons = new Image[maxBulletIcons];
        
        for (int i = 0; i < maxBulletIcons; i++)
        {
            GameObject icon = Instantiate(bulletIconPrefab, bulletIconsContainer);
            bulletIcons[i] = icon.GetComponent<Image>();
            icon.SetActive(false);
        }
    }
    
    private void UpdateWeaponDisplay()
    {
        if (weaponSwitching == null)
            return;
        
        int weaponIndex = weaponSwitching.selectedWeapon;
        Transform weaponTransform = weaponSwitching.transform.GetChild(weaponIndex);
        Shooter currentShooter = weaponTransform.GetComponent<Shooter>();
        
        if (currentShooter == null)
        {
            weaponNameText.text = "NO WEAPON";
            ammoCountText.text = "-- / --";
            return;
        }
        
        if (weaponIndex != lastWeaponIndex)
        {
            OnWeaponSwitched(weaponIndex);
            lastWeaponIndex = weaponIndex;
        }
        
        int currentAmmo = currentShooter.CurrentAmmo;
        int maxAmmo = currentShooter.MagSize;
        
        UpdateWeaponName(weaponTransform.name);
        UpdateWeaponIcon(weaponTransform.name);
        UpdateAmmoCount(currentAmmo, maxAmmo);
        UpdateBulletIcons(currentAmmo, maxAmmo);
    }
    
    private void OnWeaponSwitched(int weaponIndex)
    {
        transform.localScale = originalScale * 0.95f;
    }
    
    private void UpdateWeaponName(string weaponName)
    {
        if (weaponNameText == null)
            return;
        
        string displayName = weaponName.ToUpper().Replace("(CLONE)", "").Trim();
        
        if (displayName.Contains("PISTOL"))
            displayName = "PISTOL";
        else if (displayName.Contains("SMG"))
            displayName = "SMG";
        else if (displayName.Contains("SHOTGUN"))
            displayName = "SHOTGUN";
        
        weaponNameText.text = displayName;
    }
    
    private void UpdateWeaponIcon(string weaponName)
    {
        if (weaponIconImage == null || weaponData == null)
            return;
        
        foreach (var data in weaponData)
        {
            if (weaponName.ToUpper().Contains(data.weaponName.ToUpper()))
            {
                weaponIconImage.sprite = data.weaponIcon;
                weaponIconImage.color = Color.white;
                return;
            }
        }
        
        weaponIconImage.color = new Color(1f, 1f, 1f, 0.3f);
    }
    
    private void UpdateAmmoCount(int current, int max)
    {
        if (ammoCountText == null)
            return;
        
        ammoCountText.text = $"{current} / {max}";
        
        if (current <= lowAmmoThreshold)
            ammoCountText.color = lowAmmoColor;
        else
            ammoCountText.color = normalAmmoColor;
    }
    
    private void UpdateBulletIcons(int current, int max)
    {
        if (bulletIcons == null || bulletIcons.Length == 0)
            return;
        
        int iconsToShow = Mathf.Min(max, maxBulletIcons);
        
        for (int i = 0; i < bulletIcons.Length; i++)
        {
            if (i < iconsToShow)
            {
                bulletIcons[i].gameObject.SetActive(true);
                
                if (i < current)
                    bulletIcons[i].color = normalAmmoColor;
                else
                    bulletIcons[i].color = emptyBulletColor;
            }
            else
            {
                bulletIcons[i].gameObject.SetActive(false);
            }
        }
        
        if (transform.localScale != originalScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * weaponSwitchAnimSpeed);
        }
    }
}

[System.Serializable]
public class WeaponDisplayData
{
    public string weaponName;
    public Sprite weaponIcon;
}
