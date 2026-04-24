using UnityEngine;

namespace iglooartworks
{
    public class FanController : MonoBehaviour
    {
        public float rotationSpeed = 5f; // rotation speed in degrees
        public bool horizontal = false;

        void Update()
        {
            if (horizontal) transform.Rotate(Vector3.up * rotationSpeed, Space.Self);
            else transform.Rotate(Vector3.forward * rotationSpeed, Space.Self);
        }
    }
}