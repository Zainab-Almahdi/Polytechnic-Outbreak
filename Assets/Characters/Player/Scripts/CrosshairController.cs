using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public WeaponSwitcher weaponSwitcher;
    public GameObject crosshairUI;

    void Update()
    {
        if (weaponSwitcher == null)
            return;

        bool hasWeapon = weaponSwitcher.HasWeaponEquipped();

        crosshairUI.SetActive(hasWeapon);
    }
}