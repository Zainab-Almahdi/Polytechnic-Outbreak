using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int maxWeapons = 2;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Debug.Log("Switching weapons (Max: " + maxWeapons + ")");
        }
    }
}
