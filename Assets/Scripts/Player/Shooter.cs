using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private float fireRate = 0.3f; // Seconds between shots

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    public int CurrentAmmo => currentAmmo;
    public int MagSize => maxAmmo;

    public enum WeaponType
    {
        Pistol,
        Shotgun
    }

    [Header("Weapon Type")]
    [SerializeField] private WeaponType weaponType = WeaponType.Pistol; // This ensures pistol is default

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletsPerShot = 6;
    [SerializeField] private float spreadAngle = 8f;

    void Start()
    {
        weaponType = WeaponType.Pistol; // Force pistol as default at runtime

        currentAmmo = maxAmmo;
        // Set initial weapon model
        // if (pistolModel != null) pistolModel.SetActive(true);
        // if (shotgunModel != null) shotgunModel.SetActive(false);
    }

    void Update()
    {
        if (isReloading)
            return;

        // --- Weapon switching ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponType = WeaponType.Pistol;
            // if (pistolModel != null) pistolModel.SetActive(true);
            // if (shotgunModel != null) shotgunModel.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponType = WeaponType.Shotgun;
            // if (pistolModel != null) pistolModel.SetActive(false);
            // if (shotgunModel != null) shotgunModel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        // Optionally: play reload animation or sound here
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            // Optionally: play empty click sound here
            return;
        }

        currentAmmo--;

        if (weaponType == WeaponType.Pistol)
        {
            FireBullet(playerCamera.transform.forward);
        }
        else if (weaponType == WeaponType.Shotgun)
        {
            for (int i = 0; i < pelletsPerShot; i++)
            {
                // Calculate spread
                Vector3 spread = playerCamera.transform.forward;
                spread = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f
                ) * spread;

                FireBullet(spread);
            }
        }
    }

    void FireBullet(Vector3 direction)
    {
        Vector3 origin = playerCamera.transform.position;
        RaycastHit hit;

        // --- Aim Assist removed: just do a normal raycast ---
        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);

            if (fireEffect != null)
            {
                ParticleSystem fire = Instantiate(fireEffect, firePoint.position, firePoint.rotation, firePoint);
                fire.Play();
                Destroy(fire.gameObject, fire.main.duration);
            }

            if (hitEffect != null)
            {
                ParticleSystem hitFx = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                hitFx.Play();
                Destroy(hitFx.gameObject, hitFx.main.duration);
            }

            // Damage EnemyAiTutorial if hit
            EnemyAiTutorial enemyAI = hit.collider.GetComponent<EnemyAiTutorial>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(1); // Each pellet does 1 damage
            }
        }
        else
        {
            if (fireEffect != null)
            {
                ParticleSystem fire = Instantiate(fireEffect, firePoint.position, firePoint.rotation, firePoint);
                fire.Play();
                Destroy(fire.gameObject, fire.main.duration);
            }
        }
    }
}
