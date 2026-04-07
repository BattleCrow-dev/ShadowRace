using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YG;

public class InputManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image gasButton;
    [SerializeField] private Image breakButton;
    [SerializeField] private RectTransform steeringWheel;
    
    private float maxWheelAngle = 90f;

    private float throttle;
    private float steering;

    private bool mobile;
    private bool isDraggingWheel = false;
    private float currentWheelAngle = 0f;
    private Vector2 lastTouchPosition;

    private void Awake()
    {
        mobile = YG2.envir.isMobile;

        if (mobile)
        {
            var eventTrigger = steeringWheel.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = steeringWheel.gameObject.AddComponent<EventTrigger>();

            var pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => OnWheelPressed((PointerEventData)data));
            eventTrigger.triggers.Add(pointerDown);

            var drag = new EventTrigger.Entry();
            drag.eventID = EventTriggerType.Drag;
            drag.callback.AddListener((data) => OnWheelDrag((PointerEventData)data));
            eventTrigger.triggers.Add(drag);

            var pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => OnWheelReleased());
            eventTrigger.triggers.Add(pointerUp);
        }
    }

    public void ShowUI()
    {
        if (mobile)
            FindAnyObjectByType<UIManager>().ShowMobileUI();
        else
            FindAnyObjectByType<UIManager>().ShowPCUI();
    }

    public void OnGasPressed()
    {
        gasButton.transform.Rotate(25f, 0f, 0f);
        gasButton.color = Color.gray;
        throttle = 1f;
    }
    public void OnGasUnpressed()
    {
        gasButton.transform.Rotate(-25f, 0f, 0f);
        gasButton.color = Color.white;
        throttle = 0f;
    }

    public void OnBreakPressed()
    {
        breakButton.transform.Rotate(25f, 0f, 0f);
        breakButton.color = Color.gray;
        throttle = -0.5f;
    }
    public void OnBreakUnpressed()
    {
        breakButton.transform.Rotate(-25f, 0f, 0f);
        breakButton.color = Color.white;
        throttle = 0f;
    }

    private void Update()
    {
        if (gameManager.GetIsGameStarted())
        {
            if (!mobile)
                KeyboardInput();
        }
    }

    private void KeyboardInput()
    {
        throttle = 0f;
        steering = 0f;

        if (Keyboard.current.aKey.isPressed) steering = 1f;
        if (Keyboard.current.dKey.isPressed) steering = -1f;

        if (Keyboard.current.wKey.isPressed) throttle = 1f;
        if (Keyboard.current.sKey.isPressed) throttle = -0.5f;
    }

    private void OnWheelPressed(PointerEventData data)
    {
        isDraggingWheel = true;
        lastTouchPosition = data.position;
    }

    private void OnWheelDrag(PointerEventData data)
    {
        if (!isDraggingWheel) return;

        Vector2 delta = data.position - lastTouchPosition;
        float angleDelta = delta.x * (maxWheelAngle / Screen.width * 2f);

        currentWheelAngle += angleDelta * 2f;
        currentWheelAngle = Mathf.Clamp(currentWheelAngle, -maxWheelAngle, maxWheelAngle);

        steeringWheel.rotation = Quaternion.Euler(0f, 0f, -currentWheelAngle);

        steering = -currentWheelAngle / maxWheelAngle;

        lastTouchPosition = data.position;
    }

    private void OnWheelReleased()
    {
        isDraggingWheel = false;

        currentWheelAngle = 0f;
        steeringWheel.rotation = Quaternion.identity;
        steering = 0f;
    }

    public float GetThrottle() => throttle;
    public float GetSteering() => steering;
}
