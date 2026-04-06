using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Tracks")]
    [SerializeField] private GameObject greenTrack;
    [SerializeField] private GameObject orangeTrack;
    [SerializeField] private GameObject yellowTrack;

    [Header("Cars")]
    [SerializeField] private GameObject commonCar;
    [SerializeField] private GameObject fastCar;
    [SerializeField] private GameObject pickupCar;

    private UIManager uiManager;
    private GhostsManager ghostsManager;
    private CameraManager cameraManager;
    private InputManager inputManager;

    private float lapTimer;

    private int totalCheckpoints;
    private int passedCheckpoints;

    private int currentTrack;
    private int currentCar;
    private bool lapStarted;
    private bool isGameStarted;
    private bool isPaused = false;

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        ghostsManager = FindAnyObjectByType<GhostsManager>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        inputManager = FindAnyObjectByType<InputManager>();

        uiManager.UpdateLapTimer(lapTimer);
    }

    public void StartGame()
    {
        isGameStarted = true;
        inputManager.ShowUI();
        ghostsManager.StartGame(currentTrack, currentCar);
    }

    private void Update()
    {
        if (lapStarted && isGameStarted)
        {
            lapTimer += Time.deltaTime;
            uiManager.UpdateLapTimer(lapTimer);

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                ReloadScene();
        }
    }

    public void RegisterCheckpoint(int checkpointIndex)
    {
        if (!lapStarted) return;

        if (checkpointIndex == passedCheckpoints)
        {
            passedCheckpoints++;
        }
    }

    public void RegisterStart(int checkpointsCount)
    {
        totalCheckpoints = checkpointsCount;

        if (!lapStarted && isGameStarted)
        {
            lapStarted = true;
            lapTimer = 0f;
            passedCheckpoints = 0;
            ghostsManager.StartLap();
            return;
        }
        if (passedCheckpoints == totalCheckpoints)
        {
            CompleteLap();
        }
    }

    public void CompleteLap()
    {
        lapTimer = 0f;
        passedCheckpoints = 0;

        ghostsManager.FinishLap();
        isGameStarted = false;
        uiManager.ShowFinishPanel();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ChooseTrack(int index)
    {
        greenTrack.SetActive(false);
        orangeTrack.SetActive(false);
        yellowTrack.SetActive(false);
        currentTrack = index;

        switch (index)
        {
            case 0:
                greenTrack.SetActive(true);
                break;
            case 1:
                orangeTrack.SetActive(true);
                break;
            case 2:
                yellowTrack.SetActive(true);
                break;
            default:
                greenTrack.SetActive(true);
                break;
        }
    }

    public void ChooseCar(int index)
    {
        commonCar.SetActive(false);
        fastCar.SetActive(false);
        pickupCar.SetActive(false);
        currentCar = index;

        switch (index)
        {
            case 0:
                commonCar.SetActive(true);
                cameraManager.SetTarget(commonCar.transform);
                ghostsManager.SetPlayerCar(commonCar.transform);
                break;
            case 1:
                fastCar.SetActive(true);
                cameraManager.SetTarget(fastCar.transform);
                ghostsManager.SetPlayerCar(fastCar.transform);
                break;
            case 2:
                pickupCar.SetActive(true);
                cameraManager.SetTarget(pickupCar.transform);
                ghostsManager.SetPlayerCar(pickupCar.transform);
                break;
            default:
                commonCar.SetActive(true);
                cameraManager.SetTarget(commonCar.transform);
                ghostsManager.SetPlayerCar(commonCar.transform);
                break;
        }
    }

    public bool GetIsGameStarted() => isGameStarted;
}
