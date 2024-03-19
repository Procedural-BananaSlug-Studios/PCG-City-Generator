using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f; // Adjust the speed as needed
    public float stopSpeed = 0f; // Speed at which the car stops when not moving

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");

        if (moveInput != 0f)
        {
            // Apply forward movement when 'W' key is pressed
            rb.velocity = transform.forward * moveInput * moveSpeed;
        }
        else
        {
            // Stop the car when 'W' key is released
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, stopSpeed * Time.deltaTime);
        }

        // Rotate the car based on horizontal input (A and D keys)
        float rotateInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, rotateInput * Time.deltaTime * moveSpeed * 10f);
    }
}
