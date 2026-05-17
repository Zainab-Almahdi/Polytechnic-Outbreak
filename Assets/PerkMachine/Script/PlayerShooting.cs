using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float reloadSpeed = 1f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Bang!");
    }
}