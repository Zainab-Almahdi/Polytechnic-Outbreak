using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    private PlayerWeapons weapons;
    private PlayerPoints points;

    private void Awake()
    {
        weapons = GetComponent<PlayerWeapons>();
        points = GetComponent<PlayerPoints>();
    }

    public void PickupPowerup(string powerupName)
    {
        if (string.IsNullOrWhiteSpace(powerupName))
        {
            return;
        }

        switch (powerupName.Trim())
        {
            case "Max Ammo":
                weapons?.RefillEquippedWeaponAmmo();
                break;
            case "Nuke":
                HandleNuke();
                break;
            case "Money Drop":
                var amount = Random.Range(500, 1501);
                points?.AddPoints(amount);
                break;
        }
    }

    private void HandleNuke()
    {
        points?.AddPoints(600);
    }
}
