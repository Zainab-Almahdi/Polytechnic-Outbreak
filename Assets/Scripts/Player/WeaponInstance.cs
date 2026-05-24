using System;

[Serializable]
public class WeaponInstance
{
    public WeaponData Data;
    public int CurrentMagazineAmmo;
    public int CurrentReserveAmmo;
    public int UpgradeLevel;

    public WeaponInstance(WeaponData data)
    {
        Data = data;
        if (data != null)
        {
            CurrentMagazineAmmo = data.MagazineSize;
            CurrentReserveAmmo = data.ReserveAmmo;
        }
    }
}
