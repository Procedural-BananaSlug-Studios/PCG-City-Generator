using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private CharacterController cc;
    private Vector3 velocity;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float sensitivity = 5.0f;
    private float rotX = 0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private float jumpMult = 3.0f;
    private float gravity = -9.81f;


    private float health = 10.0f;
    private float _iTimer = 5.0f;
    public Vector3 spawn;

    private bool leftRightLeft = false;

    private PlayerState state;

    // Start is called before the first frame update
    void Start() {
        cc = GetComponent<CharacterController>();
        spawn = transform.position;
        spawn.y += 20f;
        state = PlayerState.Walking;
    }

    // Update is called once per frame
    void Update() {
        if(state == PlayerState.Walking)
            Move();
    }

    private void Move() {
        if (cc.isGrounded && velocity.y < 0f) velocity.y = 0f;

        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        rotX -= Input.GetAxis("Mouse Y") * sensitivity;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        Vector3 horizontal = transform.TransformDirection(Vector3.right) * speed * Input.GetAxis("Horizontal");
        Vector3 vertical = transform.TransformDirection(Vector3.forward) * speed * Input.GetAxis("Vertical");
        
        if(vertical.magnitude > 0f) { leftRightLeft = !leftRightLeft; }
        velocity = new Vector3(horizontal.x + vertical.x, velocity.y, horizontal.z + vertical.z);

        if (Input.GetButtonDown("Jump") && cc.isGrounded) velocity.y += Mathf.Sqrt(jumpHeight * -jumpMult * gravity);
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        if (transform.position.y < -10f)
            Respawn();
    }

    public void ChangeHealth(float amount) {
        if (_iTimer <= 0f) {
            health += amount;
            _iTimer = 1.0f;
        }
    }

    public float GetHealth() { return health; }

    public void Respawn() {
        velocity.y = 0;
        transform.position = spawn;
        health = 10.0f;
    }

    public void Begin()
    {
        state = PlayerState.Walking;
        //source.clip = warning;
        //source.pitch = 0.8f;
        //source.PlayOneShot(source.clip);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

public enum PlayerState {
    Start,
    Walking
}