using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YG;

public class InputManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Joystick joystick;

    private float throttle;
    private float steering;

    private bool mobile;

    private void Awake()
    {
        mobile = YG2.envir.isMobile;

        if (mobile)
            FindAnyObjectByType<UIManager>().ShowMobileUI();
        else
            FindAnyObjectByType<UIManager>().ShowPCUI();
    }

    private void Update()
    {
        if (mobile)
            MobileInput();
        else
            KeyboardInput();
    }

    private void MobileInput()
    {
        throttle = Mathf.Abs(joystick.Vertical) > 0.1f ? joystick.Vertical : 0f;

        if (throttle > 0f)
            steering = Mathf.Abs(joystick.Horizontal) > 0.1f ? -joystick.Horizontal : 0f;
        else
            steering = Mathf.Abs(joystick.Horizontal) > 0.1f ? joystick.Horizontal : 0f;
    }

    private void KeyboardInput()
    {
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

        if (Keyboard.current.escapeKey.isPressed)
            SceneManager.LoadScene(0);
    }

    public float GetThrottle() => throttle;
    public float GetSteering() => steering;
}
