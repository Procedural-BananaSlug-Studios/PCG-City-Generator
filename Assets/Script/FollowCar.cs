using UnityEngine;

public class FollowCar : MonoBehaviour
{
    public Transform target;    // Reference to the car's transform
    public Vector3 offset;      // Offset from the car

    private void LateUpdate()
    {
        // Set the camera's position to follow the car with the specified offset
        transform.position = target.position + offset;

        // Make the camera look at the car
        transform.LookAt(target);
    }
}
