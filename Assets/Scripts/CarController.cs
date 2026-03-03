using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    public enum DriveType { FWD, RWD, AWD }
    public DriveType driveType = DriveType.RWD;

    [Header("Engine")]
    public float engineForce = 15f;
    public float maxSpeed = 20f;

    [Header("Steering")]
    public float turnSpeed = 250f;

    [Header("Grip")]
    public float baseGrip = 8f;
    public float fwdUndersteer = 0.7f;
    public float rwdOversteer = 0.6f;

    [Header("Resistance")]
    public float drag = 1.5f;

    private Rigidbody2D rb;

    private float throttle;
    private float steering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 2f;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        throttle = 0f;
        steering = 0f;

        if (Keyboard.current.aKey.isPressed) steering = 1f;
        if (Keyboard.current.dKey.isPressed) steering = -1f;

        if (Keyboard.current.wKey.isPressed) throttle = 1f;

        if (Keyboard.current.sKey.isPressed)
        {
            throttle = -0.5f;
            steering *= -1f;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) driveType = DriveType.FWD;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) driveType = DriveType.RWD;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) driveType = DriveType.AWD;
    }

    private void FixedUpdate()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float forwardSpeed = Vector2.Dot(rb.linearVelocity, forward);
        float sideSpeed = Vector2.Dot(rb.linearVelocity, right);

        Vector2 forwardVel = forward * forwardSpeed;
        Vector2 sideVel = right * sideSpeed;

        float grip = baseGrip;

        if (driveType == DriveType.FWD && Mathf.Abs(throttle) > 0.1f)
            grip *= fwdUndersteer;

        if (driveType == DriveType.RWD && Mathf.Abs(throttle) > 0.1f)
            grip *= rwdOversteer;

        rb.linearVelocity = forwardVel + sideVel / grip;

        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(forward * throttle * engineForce, ForceMode2D.Force);

        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        rb.MoveRotation(rb.rotation + steering * turnSpeed * speedFactor * Time.fixedDeltaTime);

        rb.linearVelocity *= 1f / (1f + drag * Time.fixedDeltaTime);
    }
}