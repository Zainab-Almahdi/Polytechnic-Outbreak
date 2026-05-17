using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeapon = -1;

    void Start()
    {
        EquipWeapon(-1); // start with no weapon
    }

    void Update()
    {
        HandleNumberKeys();
        HandleScrollWheel();
    }

    void HandleNumberKeys()
    {
        for (int i = 0; i < weapons.Length && i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha1 + i)))
            {
                EquipWeapon(i);
            }
        }
    }

    void HandleScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            EquipWeapon((currentWeapon + 1) % weapons.Length);
        }
        else if (scroll < 0f)
        {
            EquipWeapon((currentWeapon - 1 + weapons.Length) % weapons.Length);
        }
    }

    void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        if (index >= 0 && index < weapons.Length)
        {
            weapons[index].SetActive(true);
            currentWeapon = index;
        }
        else
        {
            currentWeapon = -1;
        }
    }
}