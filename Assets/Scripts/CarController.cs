using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    public float engineForce = 15f;
    public float maxSpeed = 20f;
    public float turnSpeed = 250f;
    public float grip = 8f;
    public float drag = 1.5f;

    private Rigidbody2D rb;
    private float throttle;
    private float steering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        throttle = 0f;
        steering = 0f;

        if (Keyboard.current.wKey.isPressed) throttle = 1f;
        if (Keyboard.current.sKey.isPressed) throttle = -1f;
        if (Keyboard.current.aKey.isPressed) steering = 1f;
        if (Keyboard.current.dKey.isPressed) steering = -1f;
    }

    private void FixedUpdate()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        Vector2 forwardVel = forward * Vector2.Dot(rb.linearVelocity, forward);
        Vector2 sideVel = right * Vector2.Dot(rb.linearVelocity, right);

        rb.linearVelocity = forwardVel + sideVel / grip;

        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(forward * throttle * engineForce, ForceMode2D.Force);

        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        rb.MoveRotation(rb.rotation + steering * turnSpeed * speedFactor * Time.fixedDeltaTime);

        rb.linearVelocity *= 1f / (1f + drag * Time.fixedDeltaTime);
    }
}