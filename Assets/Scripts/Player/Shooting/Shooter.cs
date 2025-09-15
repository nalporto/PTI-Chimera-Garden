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

    // Animator bool names
    private const string ANIM_IS_SWITCHED_IN = "IsSwitchedIn";
    private const string ANIM_IS_SHOOTING = "IsShooting";
    private const string ANIM_IS_RELOADING = "IsReloading";

    // timing
    [SerializeField] private float switchInDelay = 0.3f;
    private Coroutine switchInCoroutine;
    private Coroutine resetShootingCoroutine;
    [SerializeField] private float shootingBoolDuration = 0.12f;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Coroutine reloadCoroutine; // track reload coroutine

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

        // try auto-assign animator if null
        if (weaponAnimator == null)
            weaponAnimator = GetComponentInChildren<Animator>();

        // initialize animator bools
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool(ANIM_IS_SWITCHED_IN, false); // start false
            weaponAnimator.SetBool(ANIM_IS_SHOOTING, false);
            weaponAnimator.SetBool(ANIM_IS_RELOADING, false);
        }

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
            // start and track reload coroutine so we can stop it if needed
            if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
            reloadCoroutine = StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Call when weapon is equipped/switched in
    public void SwitchIn()
    {
        if (weaponAnimator != null)
            weaponAnimator.SetBool(ANIM_IS_SWITCHED_IN, false);

        if (switchInCoroutine != null)
            StopCoroutine(switchInCoroutine);
        switchInCoroutine = StartCoroutine(DelayedSetSwitchedIn(switchInDelay));
    }

    // Call when weapon is switched out
    public void SwitchOut()
    {
        if (switchInCoroutine != null)
        {
            StopCoroutine(switchInCoroutine);
            switchInCoroutine = null;
        }

        if (resetShootingCoroutine != null)
        {
            StopCoroutine(resetShootingCoroutine);
            resetShootingCoroutine = null;
        }

        // stop an in-progress reload and ensure animator bool cleared
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
        if (weaponAnimator != null)
            weaponAnimator.SetBool(ANIM_IS_RELOADING, false);

        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool(ANIM_IS_SWITCHED_IN, false);
            weaponAnimator.SetBool(ANIM_IS_SHOOTING, false);
        }
    }

    private IEnumerator DelayedSetSwitchedIn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (weaponAnimator != null)
            weaponAnimator.SetBool(ANIM_IS_SWITCHED_IN, true);
        switchInCoroutine = null;
    }

    System.Collections.IEnumerator Reload()
    {
        // prevent reload when at full ammo
        if (currentAmmo >= maxAmmo)
            yield break;

        // ensure previous reload coroutine reference cleared on exit
        isReloading = true;
        if (weaponAnimator != null)
            weaponAnimator.SetBool(ANIM_IS_RELOADING, true);

        try
        {
            // use realtime so timescale changes don't prematurely finish reload
            if (reloadSFX != null && audioSource != null)
                audioSource.PlayOneShot(reloadSFX);

            if (weaponAnimator != null && !string.IsNullOrEmpty(reloadAnimTrigger))
                weaponAnimator.SetTrigger(reloadAnimTrigger);

            yield return new WaitForSecondsRealtime(reloadTime);

            currentAmmo = maxAmmo;
        }
        finally
        {
            // always clear reloading state and animator bool even if coroutine stopped
            isReloading = false;
            if (weaponAnimator != null)
                weaponAnimator.SetBool(ANIM_IS_RELOADING, false);
            reloadCoroutine = null;
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            // Optionally: play empty click sound here
            return;
        }

        currentAmmo--;

        // Play shoot animation trigger if assigned
        if (weaponAnimator != null && !string.IsNullOrEmpty(shootAnimTrigger))
            weaponAnimator.SetTrigger(shootAnimTrigger);

        // Set IsShooting bool briefly
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool(ANIM_IS_SHOOTING, true);
            if (resetShootingCoroutine != null)
                StopCoroutine(resetShootingCoroutine);
            resetShootingCoroutine = StartCoroutine(ResetShootingBool(shootingBoolDuration));
        }

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

    private IEnumerator ResetShootingBool(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (weaponAnimator != null)
            weaponAnimator.SetBool(ANIM_IS_SHOOTING, false);
        resetShootingCoroutine = null;
    }

    void FireBullet(Vector3 direction)
    {
        Vector3 origin = playerCamera.transform.position;
        RaycastHit hit;
        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);
            Debug.Log($"Bullet hit: {hit.collider.name} (tag: {hit.collider.tag})"); // debug

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

            // find EnemyAiTutorial on the hit collider or its parents
            EnemyAiTutorial enemyAI = hit.collider.GetComponentInParent<EnemyAiTutorial>();
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
        if (hitMarker == null) yield break;
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