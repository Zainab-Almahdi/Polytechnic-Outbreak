using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();

    private int currentWeapon = -1;

    void Start()
    {
        CleanWeapons();

        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    void Update()
    {
        CleanWeapons();

        if (weapons.Count == 0) return;

        HandleInput();
    }

    void HandleInput()
    {
        HandleNumberKeys();
        HandleScrollWheel();
    }

    void HandleNumberKeys()
    {
        for (int i = 0; i < weapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                EquipWeapon(i);
        }
    }

    void HandleScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            currentWeapon++;
            if (currentWeapon >= weapons.Count)
                currentWeapon = 0;

            EquipWeapon(currentWeapon);
        }
        else if (scroll < 0f)
        {
            currentWeapon--;
            if (currentWeapon < 0)
                currentWeapon = weapons.Count - 1;

            EquipWeapon(currentWeapon);
        }
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        CleanWeapons();

        // Prevent duplicates
        foreach (GameObject w in weapons)
        {
            if (w == null) continue;

            string existingName = w.name.Replace("(Clone)", "");
            if (existingName == weaponPrefab.name)
                return;
        }

        GameObject newWeapon = Instantiate(weaponPrefab, transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        newWeapon.SetActive(false);
        weapons.Add(newWeapon);

        EquipWeapon(weapons.Count - 1);
    }

    public void EquipWeapon(int index)
    {
        CleanWeapons();

        if (weapons.Count == 0) return;
        if (index < 0 || index >= weapons.Count) return;

        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
                weapons[i].SetActive(false);
        }

        weapons[index].SetActive(true);
        currentWeapon = index;
    }

    public GameObject GetCurrentWeapon()
    {
        CleanWeapons();

        if (currentWeapon < 0 || currentWeapon >= weapons.Count)
            return null;

        return weapons[currentWeapon];
    }

    public bool HasWeaponEquipped()
    {
        return GetCurrentWeapon() != null;
    }

    void CleanWeapons()
    {
        weapons.RemoveAll(w => w == null);
    }
}