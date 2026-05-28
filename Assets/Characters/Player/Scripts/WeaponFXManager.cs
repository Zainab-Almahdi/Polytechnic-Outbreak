using UnityEngine;

public class WeaponFXManager : MonoBehaviour
{
    public static WeaponFXManager Instance;

    public ParticleSystem muzzleFlashPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void PlayMuzzleFlash(Transform muzzlePoint)
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null)
            return;

        ParticleSystem fx = Instantiate(
            muzzleFlashPrefab,
            muzzlePoint.position,
            muzzlePoint.rotation
        );

        fx.Play();

        Destroy(fx.gameObject, 1f);
    }
}