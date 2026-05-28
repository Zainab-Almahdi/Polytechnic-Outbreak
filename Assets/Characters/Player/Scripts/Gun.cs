using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptySound;

    private PlayerInputHandler inputHandler;
    private PlayerWeapons playerWeapons;
    private WeaponInstance weaponInstance;

    [SerializeField] private Transform muzzlePoint;

    public Camera playerCamera;
    public GameObject impactEffect;

    public float damage = 25f;
    public float headshotMultiplier = 2f;
    public float range = 100f;

    public float fireRate = 0.1f;

    public int magazineSize = 30;
    public int reserveAmmo = 210;
    public float reloadTime = 2.5f;

    private bool isReloading;
    private bool canShoot = true;

    public bool isBurstWeapon = false;
    public int burstCount = 3;

    public bool isShotgun = false;
    public int pelletCount = 8;
    public float spreadAngle = 5f;

    [Header("Recoil")]
    public float recoilX = 1.5f;
    public float recoilY = 2.5f;
    public float recoilReturnSpeed = 8f;
    public float recoilSnappiness = 12f;
    public float recoilHoldTime = 0.05f;

    private Vector2 currentRecoil;
    private Vector2 targetRecoil;
    private float lastShotTime;

    public void Initialize(WeaponInstance instance, PlayerWeapons manager)
    {
        weaponInstance = instance;
        playerWeapons = manager;

        if (weaponInstance != null)
        {
            damage = weaponInstance.Damage;
            headshotMultiplier = weaponInstance.HeadshotMultiplier;
            range = weaponInstance.Range;
            reloadTime = weaponInstance.ReloadSpeedSeconds;
            magazineSize = weaponInstance.MagazineSize;
            reserveAmmo = weaponInstance.ReserveAmmo;
            isBurstWeapon = weaponInstance.IsBurstWeapon;
            burstCount = weaponInstance.BurstCount;
            isShotgun = weaponInstance.IsShotgun;
            pelletCount = weaponInstance.PelletCount;
            spreadAngle = weaponInstance.SpreadAngle;

            if (weaponInstance.FireRateRpm > 0)
                fireRate = 60f / weaponInstance.FireRateRpm;
        }
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        inputHandler = GetComponentInParent<PlayerInputHandler>();
    }

    void Update()
    {
        if (weaponInstance == null || inputHandler == null)
            return;

        if (isReloading)
        {
            UpdateRecoil();
            return;
        }

        if (inputHandler.ReloadPressed)
        {
            StartCoroutine(Reload());
            return;
        }

        if (inputHandler.ShootPressed && canShoot)
        {
            if (weaponInstance.CurrentMagazineAmmo <= 0)
            {
                audioSource.PlayOneShot(emptySound);
                StartCoroutine(Reload());
                return;
            }

            StartCoroutine(ShootRoutine());
        }

        UpdateRecoil(); 
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;

        if (isBurstWeapon)
        {
            for (int i = 0; i < burstCount; i++)
            {
                FireShot();
                yield return new WaitForSeconds(0.08f);
            }
        }
        else
        {
            FireShot();
        }

        weaponInstance.CurrentMagazineAmmo--;
        NotifyAmmoChanged();

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    void FireShot()
    {
        WeaponFXManager.Instance?.PlayMuzzleFlash(muzzlePoint);
        audioSource.PlayOneShot(shootSound);

        ApplyRecoil();

        if (isShotgun)
        {
            for (int i = 0; i < pelletCount; i++)
                FireRay(GetShotgunDirection());
        }
        else
        {
            FireRay(GetRecoilAdjustedDirection());
        }

        lastShotTime = Time.time;
    }

    void ApplyRecoil()
    {
        targetRecoil += new Vector2(
            recoilX,
            Random.Range(-recoilY, recoilY)
        );
    }

    void UpdateRecoil()
    {
        currentRecoil = Vector2.Lerp(
            currentRecoil,
            targetRecoil,
            recoilSnappiness * Time.deltaTime
        );

        if (Time.time - lastShotTime > recoilHoldTime)
        {
            targetRecoil = Vector2.Lerp(
                targetRecoil,
                Vector2.zero,
                recoilReturnSpeed * Time.deltaTime
            );
        }

        PlayerCameraLook cam = playerCamera.GetComponentInParent<PlayerCameraLook>();
        if (cam != null)
        {
            cam.recoilOffset = currentRecoil;
        }
    }

    Vector3 GetRecoilAdjustedDirection()
    {
        Vector3 dir = playerCamera.transform.forward;

        dir += playerCamera.transform.up * (-currentRecoil.y * 0.01f);
        dir += playerCamera.transform.right * (currentRecoil.x * 0.01f);

        return dir.normalized;
    }

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        if (weaponInstance.CurrentReserveAmmo <= 0)
            yield break;

        if (weaponInstance.CurrentMagazineAmmo == magazineSize)
            yield break;

        isReloading = true;
        audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = magazineSize - weaponInstance.CurrentMagazineAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, weaponInstance.CurrentReserveAmmo);

        weaponInstance.CurrentMagazineAmmo += ammoToLoad;
        weaponInstance.CurrentReserveAmmo -= ammoToLoad;

        NotifyAmmoChanged();
        isReloading = false;
    }

    void NotifyAmmoChanged()
    {
        playerWeapons?.InvokeAmmoChanged(
            weaponInstance.CurrentMagazineAmmo,
            weaponInstance.CurrentReserveAmmo
        );
    }

    void FireRay(Vector3 direction)
    {
        Ray ray = new Ray(playerCamera.transform.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            ZombieHealth zombie = hit.collider.GetComponentInParent<ZombieHealth>();

            if (zombie != null)
            {
                float finalDamage = damage;

                if (hit.collider.CompareTag("Head"))
                    finalDamage *= headshotMultiplier;

                zombie.TakeDamage(finalDamage);
            }

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                Destroy(impact, 0.3f);
            }
        }
    }

    Vector3 GetShotgunDirection()
    {
        Vector3 direction = playerCamera.transform.forward;
        direction += Random.insideUnitSphere * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        return direction.normalized;
    }
}