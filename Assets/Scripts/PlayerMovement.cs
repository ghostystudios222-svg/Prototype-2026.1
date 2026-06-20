using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public float jumpForce;
    public InputActionReference move;
    public InputActionReference jump;

    private Vector2 _moveDirection;
    private bool isGrounded;
    

    // Update is called once per frame
    void Update()
    {
        // Check if grounded (adjust the final float value based on player height)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Read movement input
        _moveDirection = move.action.ReadValue<Vector2>();

        // Jump input, added a ground check to prevent double jumping
        if (jump.action.WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void FixedUpdate()
    {
        // Calculate movement vector based on input and character orientation
        Vector3 moveVector = (transform.forward * _moveDirection.y + transform.right * _moveDirection.x).normalized;
        Vector3 targetVelocity = new Vector3(moveVector.x * moveSpeed, rb.linearVelocity.y, moveVector.z * moveSpeed);
        Vector3 velocityDelta = targetVelocity - rb.linearVelocity;
        rb.AddForce(velocityDelta, ForceMode.VelocityChange);

        // Extra gravity to make the player fall faster
        rb.AddForce(Vector3.down * 5f, ForceMode.Acceleration);
    }
}
