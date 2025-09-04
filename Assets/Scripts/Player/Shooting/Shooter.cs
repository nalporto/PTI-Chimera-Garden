using UnityEngine;
using System.Collections;

public enum WeaponType
{
    PISTOL,
    SHOTGUN
}

public class Shooter : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private float fireRate = 0.3f; // Seconds between shots
    [SerializeField] private float damage = 1f; // Damage per shot

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private RuntimeAnimatorController weaponAnimatorController; // Assign in Inspector
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private string shootAnimTrigger = "Shoot";
    [SerializeField] private string reloadAnimTrigger = "Reload";

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    public int CurrentAmmo => currentAmmo;
    public int MagSize => maxAmmo;
    public bool IsReloading => isReloading;
    public float Damage => damage;

    [Header("Weapon Type")]
    [SerializeField] private WeaponType weaponType = WeaponType.PISTOL; // Dropdown in inspector

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletsPerShot = 8; // 8 pellets for shotgun
    [SerializeField] private float spreadRadius = 0.15f; // Spread radius for shotgun (meters)

    [SerializeField] private TrailRenderer bulletTrailPrefab;
    [SerializeField] private ParticleSystem impactParticleSystem;

    [Header("UI")]
    [SerializeField] private GameObject hitMarker; // Assign your hit marker GameObject in the Inspector
    [SerializeField] private float hitMarkerDisplayTime = 0.07f; // How long the hit marker is shown

    [SerializeField] private SimpleDynamicCrosshair reticle; // Assign in Inspector

    void Start()
    {
        currentAmmo = maxAmmo;

        // Assign animator controller if provided
        if (weaponAnimator != null && weaponAnimatorController != null)
            weaponAnimator.runtimeAnimatorController = weaponAnimatorController;

        // Try to get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Ensure hit marker is off at start
        if (hitMarker != null)
            hitMarker.SetActive(false);
    }

    void Update()
    {
        if (isReloading)
            return;

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

        // Play reload animation if assigned
        if (weaponAnimator != null && !string.IsNullOrEmpty(reloadAnimTrigger))
            weaponAnimator.SetTrigger(reloadAnimTrigger);

        // Play reload SFX if assigned
        if (reloadSFX != null && audioSource != null)
            audioSource.PlayOneShot(reloadSFX);

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

        // Play shoot animation if assigned
        if (weaponAnimator != null && !string.IsNullOrEmpty(shootAnimTrigger))
            weaponAnimator.SetTrigger(shootAnimTrigger);

        // Play shoot SFX if assigned
        if (shootSFX != null && audioSource != null)
            audioSource.PlayOneShot(shootSFX);

        if (weaponType == WeaponType.PISTOL)
        {
            FireBullet(playerCamera.transform.forward);
        }
        else if (weaponType == WeaponType.SHOTGUN)
        {
            Vector3 origin = playerCamera.transform.position;
            Vector3 forward = playerCamera.transform.forward;
            for (int i = 0; i < pelletsPerShot; i++)
            {
                // Random point in circle for spread
                Vector2 circle = Random.insideUnitCircle * spreadRadius;
                Vector3 spreadDir = (forward + playerCamera.transform.right * circle.x + playerCamera.transform.up * circle.y).normalized;
                FireBullet(spreadDir);
            }
        }
    }

    void FireBullet(Vector3 direction)
    {
        Vector3 origin = playerCamera.transform.position;
        RaycastHit hit;
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

            // Spawn bullet trail
            if (bulletTrailPrefab != null)
            {
                TrailRenderer trail = Instantiate(bulletTrailPrefab, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
            }

            // Damage EnemyAiTutorial if hit
            EnemyAiTutorial enemyAI = hit.collider.GetComponent<EnemyAiTutorial>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);

                // Show hit marker
                if (hitMarker != null)
                    StartCoroutine(ShowHitMarker());
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

            // Spawn bullet trail to max distance
            if (bulletTrailPrefab != null)
            {
                Vector3 endPoint = ray.origin + ray.direction * 100f;
                RaycastHit fakeHit = new RaycastHit { point = endPoint, normal = ray.direction };
                TrailRenderer trail = Instantiate(bulletTrailPrefab, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, fakeHit));
            }
        }
    }

    private IEnumerator ShowHitMarker()
    {
        hitMarker.SetActive(true);
        yield return new WaitForSeconds(hitMarkerDisplayTime);
        hitMarker.SetActive(false);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        if (impactParticleSystem != null)
            Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(trail.gameObject, trail.time);
    }
}