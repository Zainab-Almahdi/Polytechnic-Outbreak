using UnityEngine;

public class Gun : MonoBehaviour
{
    MouseLook mouseLook;  

    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public float range = 100f;
    public float damage = 25f;
    public float headshotMultiplier = 2f; 

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera still missing!");
            return;
        }

        mouseLook = playerCamera.GetComponentInParent<MouseLook>();

        if (mouseLook == null)
        {
            Debug.LogError("MouseLook not found on CameraHolder!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        mouseLook.AddRecoil();

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            ZombieHealth zombie = hit.collider.GetComponentInParent<ZombieHealth>();

            if (zombie != null)
            {
                float finalDamage = damage;

                if (hit.collider.CompareTag("Head"))
                {
                    finalDamage *= headshotMultiplier;
                }

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
}
