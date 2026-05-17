using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmount = 0.02f;
    public float swaySmooth = 6f;

    public float rotationSwayAmount = 2f;
    public float rotationSmooth = 6f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        // Store the REAL starting transform
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 targetPos = new Vector3(
            -mouseX,
            -mouseY,
            0
        ) * swayAmount;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            originalPos + targetPos,
            Time.deltaTime * swaySmooth
        );

        Quaternion targetRot = Quaternion.Euler(
            -mouseY * rotationSwayAmount,
            mouseX * rotationSwayAmount,
            0
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            originalRot * targetRot,
            Time.deltaTime * rotationSmooth
        );
    }
}