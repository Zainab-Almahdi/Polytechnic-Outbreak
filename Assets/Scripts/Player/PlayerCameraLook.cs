using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform yawTarget;
    [SerializeField] private float lookSensitivityMultiplier = 1f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursor = true;

    private float xRotation;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponent<Camera>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (yawTarget == null)
        {
            if (playerCamera != null)
            {
                yawTarget = playerCamera.transform.parent != null
                    ? playerCamera.transform.parent
                    : playerCamera.transform;
            }
            else if (playerBody != null)
            {
                yawTarget = playerBody;
            }
            else
            {
                yawTarget = transform;
            }
        }
    }

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        var mouseX = Input.GetAxis("Mouse X") * lookSensitivityMultiplier;
        var mouseY = Input.GetAxis("Mouse Y") * lookSensitivityMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxPitch, maxPitch);
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (yawTarget != null)
        {
            yawTarget.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.Rotate(Vector3.up * mouseX, Space.World);
        }
    }
}
