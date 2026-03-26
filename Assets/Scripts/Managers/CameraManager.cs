using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform target;
    [SerializeField] private GameManager gameManager;

    [Header("Parametera")]
    [SerializeField] private float positionSmooth = 0.12f;
    [SerializeField] private float rotationSmooth = 6f;
    [SerializeField] private float baseSize = 6f;
    [SerializeField] private float maxZoomOut = 2f;
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float maxSpeedForZoom = 20f;

    private Vector3 velocity;
    private Rigidbody2D targetRB;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetRB = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (!target || !gameManager.GetIsGameStarted()) return;

        Vector3 targetPos = new(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, positionSmooth);

        Quaternion desiredRot = Quaternion.Euler(0f, 0f, target.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotationSmooth * Time.deltaTime);

        float targetSize = baseSize + Mathf.Clamp01(targetRB.linearVelocity.magnitude / maxSpeedForZoom) * maxZoomOut;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }
}
