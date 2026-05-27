using UnityEngine;
using Assets.UI.Scripts;

public class MouseLook : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    public float mouseSensitivity = 1f;

    [Header("Recoil")]
public float recoilKick = 2f;
    public float recoilReturnSpeed = 10f;

    private float xRotation = 0f;
    private float recoilX = 0f;
    private float recoilY = 0f;

    public Transform playerBody;

    void Start()
    {
        inputHandler = GetComponentInParent<PlayerInputHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (inputHandler == null) return;

        float sensitivity = PlayerStorageHandler.GetMouseSensitivity();
        float mouseX = inputHandler.LookInput.x * mouseSensitivity * sensitivity;
        float mouseY = inputHandler.LookInput.y * mouseSensitivity * sensitivity;

        // Permanent look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Recoil recovery
        recoilX = Mathf.Lerp(recoilX, 0f, recoilReturnSpeed * Time.deltaTime);
        recoilY = Mathf.Lerp(recoilY, 0f, recoilReturnSpeed * Time.deltaTime);

        float finalX = xRotation - recoilX;

        // Apply rotations safely
        transform.localRotation = Quaternion.Euler(finalX, 0f, 0f);
        playerBody.Rotate(Vector3.up * (mouseX + recoilY));
    }

    public void AddRecoil()
    {
        recoilX += recoilKick;
        recoilY += Random.Range(-recoilKick * 0.3f, recoilKick * 0.3f);
    }
}