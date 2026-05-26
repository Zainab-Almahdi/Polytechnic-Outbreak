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
            case "MaxAmmoPowerup":
                weapons?.RefillEquippedWeaponAmmo();
                break;
            case "NukePowerup":
                HandleNuke();
                break;
            case "MoneyDropPowerup":
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
