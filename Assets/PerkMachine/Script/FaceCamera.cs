using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        // Make text always face the camera
        transform.LookAt(Camera.main.transform);
        // Flip it around so text isn't backwards
        transform.Rotate(0, 180, 0);
    }
}