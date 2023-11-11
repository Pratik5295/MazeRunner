using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public float rotationSpeed = 45f; // Adjust the speed in the Unity Inspector.

    void Update()
    {
        // Rotate the object around its own axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
