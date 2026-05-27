using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerWeapons playerWeapons;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerWeapons = GetComponent<PlayerWeapons>();
    }

    private void Update()
    {
        if (inputHandler == null || playerWeapons == null)
        {
            return;
        }

        if (inputHandler.SwitchWeaponPressed)
        {
            playerWeapons.EquipNextWeapon();
        }
    }
}
