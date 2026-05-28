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

        string name = powerupName.Trim();
        switch (name)
        {
            case "MaxAmmoPowerup":
                weapons?.RefillAllWeaponsAmmo();
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
        points?.AddPoints(400);
        
        ZombieHealth[] zombies = Object.FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);
        foreach (var zombie in zombies)
        {
            zombie.TakeDamage(9999);
        }
    }
}
