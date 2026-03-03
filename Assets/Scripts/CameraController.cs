using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Rigidbody2D targetRb;

    [Header("Follow")]
    public float positionSmooth = 0.12f;
    public float rotationSmooth = 6f;

    [Header("Zoom")]
    public float baseSize = 6f;
    public float maxZoomOut = 2f;
    public float zoomSpeed = 3f;
    public float maxSpeedForZoom = 20f;

    private Vector3 velocity;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (!target || !targetRb) return;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            positionSmooth
        );

        Quaternion desiredRot = Quaternion.Euler(
            0f,
            0f,
            target.eulerAngles.z
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            desiredRot,
            rotationSmooth * Time.deltaTime
        );

        float speed = targetRb.linearVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(speed / maxSpeedForZoom);

        float targetSize = baseSize + speedFactor * maxZoomOut;

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }
}