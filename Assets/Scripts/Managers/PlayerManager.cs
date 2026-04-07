using UnityEngine;
using UnityEngine.InputSystem;
using YG;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    public enum DriveType { FWD, RWD, AWD }

    [Header("Car_Parameters")]
    [SerializeField] private DriveType driveType = DriveType.RWD;
    [SerializeField] private float engineForce = 15f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float turnSpeed = 250f;
    [SerializeField] private float baseGrip = 8f;
    [SerializeField] private float fwdUndersteer = 0.7f;
    [SerializeField] private float rwdOversteer = 0.6f;
    [SerializeField] private float drag = 1.5f;

    private Rigidbody2D rb;
    private InputManager inputManager;
    private GameManager gameManager;

    private float throttle;
    private float steering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputManager = FindAnyObjectByType<InputManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        throttle = inputManager.GetThrottle();
        steering = inputManager.GetSteering();
    }

    private void FixedUpdate()
    {
        if (gameManager.GetIsGameStarted())
        {
            Vector2 forward = transform.up;
            Vector2 right = transform.right;

            float forwardSpeed = Vector2.Dot(rb.linearVelocity, forward);
            float sideSpeed = Vector2.Dot(rb.linearVelocity, right);

            Vector2 forwardVel = forward * forwardSpeed;
            Vector2 sideVel = right * sideSpeed;

            if (forwardSpeed < 0f)
                steering = -steering;

            float grip = baseGrip;

            if (driveType == DriveType.FWD && Mathf.Abs(throttle) > 0.1f)
                grip *= fwdUndersteer;

            if (driveType == DriveType.RWD && Mathf.Abs(throttle) > 0.1f)
                grip *= rwdOversteer;

            rb.linearVelocity = forwardVel + sideVel / grip;

            if (rb.linearVelocity.magnitude < maxSpeed)
                rb.AddForce(engineForce * throttle * forward, ForceMode2D.Force);

            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
            rb.MoveRotation(rb.rotation + steering * turnSpeed * speedFactor * Time.fixedDeltaTime);

            rb.linearVelocity *= 1f / (1f + drag * Time.fixedDeltaTime);
        }
        else
            rb.linearVelocity = Vector2.zero;
    }
}
