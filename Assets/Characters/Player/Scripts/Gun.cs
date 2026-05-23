using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    // REFERENCES
    MouseLook mouseLook;

    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    // DAMAGE
    public float damage = 25f;
    public float headshotMultiplier = 2f;
    public float range = 100f;

    // FIRE SETTINGS
    public float fireRate = 0.1f;          // Time between shots
    private float nextTimeToFire = 0f;

    // AMMO
    public int magazineSize = 30;
    public int reserveAmmo = 210;
    public float reloadTime = 2.5f;

    private int currentAmmo;
    private bool isReloading = false;

    // BURST FIRE
    public bool isBurstWeapon = false;
    public int burstCount = 3;

    // SHOTGUN
    public bool isShotgun = false;
    public int pelletCount = 8;
    public float spreadAngle = 5f;

    void Start()
    {
        currentAmmo = magazineSize;

        if (playerCamera == null)
            playerCamera = Camera.main;

        mouseLook = playerCamera.GetComponentInParent<MouseLook>();
    }

    void Update()
    {
        if (isReloading)
            return;

        // RELOAD
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        // FIRE
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextTimeToFire)
        {
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            nextTimeToFire = Time.time + fireRate;

            if (isBurstWeapon)
                StartCoroutine(BurstFire());
            else
                Shoot();

            currentAmmo--;
        }
    }

    IEnumerator Reload()
    {
        if (reserveAmmo <= 0 || currentAmmo == magazineSize)
            yield break;

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
    }

    IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo <= 0)
                yield break;

            Shoot();
            currentAmmo--;
            yield return new WaitForSeconds(0.08f);
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        mouseLook.AddRecoil();

        if (isShotgun)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                FireRay(GetShotgunDirection());
            }
        }
        else
        {
            FireRay(playerCamera.transform.forward);
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

            GameObject impact = Instantiate(
                impactEffect,
                hit.point,
                Quaternion.LookRotation(hit.normal)
            );

            Destroy(impact, 0.3f);
        }
    }

    Vector3 GetShotgunDirection()
    {
        Vector3 direction = playerCamera.transform.forward;
        direction += Random.insideUnitSphere * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        return direction.normalized;
    }
}