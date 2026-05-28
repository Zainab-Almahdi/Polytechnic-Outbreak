using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject crosshairUI;

    void Update()
    {
        Gun gun = FindAnyObjectByType<Gun>();

        crosshairUI.SetActive(gun != null && gun.isActiveAndEnabled);
    }
}