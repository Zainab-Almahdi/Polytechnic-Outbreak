using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    private MouseLook mouseLook;
    private PlayerInputHandler inputHandler;
    private PlayerWeapons playerWeapons;
    private WeaponInstance weaponInstance;

    [SerializeField] private Transform muzzlePoint;

    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public float damage = 25f;
    public float headshotMultiplier = 2f;
    public float range = 100f;

    public float fireRate = 0.1f;
    private float nextTimeToFire = 0f;

    public int magazineSize = 30;
    public int reserveAmmo = 210;
    public float reloadTime = 2.5f;

    private bool isReloading = false;

    public bool isBurstWeapon = false;
    public int burstCount = 3;

    public bool isShotgun = false;
    public int pelletCount = 8;
    public float spreadAngle = 5f;

    public void Initialize(WeaponInstance instance, PlayerWeapons manager)
    {
        weaponInstance = instance;
        playerWeapons = manager;

        // Sync initial values if instance is valid
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
            
            // Note: FireRate logic in WeaponInstance is RPM, but Gun uses seconds between shots.
            // If RPM is set, use it.
            if (weaponInstance.FireRateRpm > 0)
            {
                fireRate = 60f / weaponInstance.FireRateRpm;
            }
        }
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
            playerCamera = GetComponentInParent<Camera>();

        if (playerCamera != null)
            mouseLook = playerCamera.GetComponentInParent<MouseLook>();

        inputHandler = GetComponentInParent<PlayerInputHandler>();
    }

    void Update()
    {
        if (isReloading || weaponInstance == null || inputHandler == null) return;

        // RELOAD
        if (inputHandler.ReloadPressed)
        {
            StartCoroutine(Reload());
            return;
        }

        // FIRE
        if (inputHandler.ShootPressed && Time.time >= nextTimeToFire)
        {
            if (weaponInstance.CurrentMagazineAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            nextTimeToFire = Time.time + fireRate;

            if (isBurstWeapon)
                StartCoroutine(BurstFire());
            else
            {
                Shoot();
                ConsumeAmmo();
            }
        }
    }

    IEnumerator Reload()
    {
        if (weaponInstance.CurrentReserveAmmo <= 0 || weaponInstance.CurrentMagazineAmmo == weaponInstance.MagazineSize)
            yield break;

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = weaponInstance.MagazineSize - weaponInstance.CurrentMagazineAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, weaponInstance.CurrentReserveAmmo);

        weaponInstance.CurrentMagazineAmmo += ammoToLoad;
        weaponInstance.CurrentReserveAmmo -= ammoToLoad;
        
        NotifyAmmoChanged();

        isReloading = false;
    }

    IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (weaponInstance.CurrentMagazineAmmo <= 0)
                yield break;

            Shoot();
            ConsumeAmmo();
            yield return new WaitForSeconds(0.08f);
        }
    }

    private void ConsumeAmmo()
    {
        weaponInstance.CurrentMagazineAmmo--;
        NotifyAmmoChanged();
    }

    void Shoot()
    {
        if (WeaponFXManager.Instance != null)
        {
            WeaponFXManager.Instance.PlayMuzzleFlash(muzzlePoint);
        }

        if (mouseLook != null)
            mouseLook.AddRecoil();

        if (isShotgun)
        {
            for (int i = 0; i < pelletCount; i++)
                FireRay(GetShotgunDirection());
        }
        else
        {
            FireRay(playerCamera.transform.forward);
        }
    }

    private void NotifyAmmoChanged()
    {
        if (playerWeapons != null)
        {
            // We need to trigger the event in PlayerWeapons so Player.cs picks it up.
            // I will add a public method to PlayerWeapons for this.
            playerWeapons.InvokeAmmoChanged(weaponInstance.CurrentMagazineAmmo, weaponInstance.CurrentReserveAmmo);
        }
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
