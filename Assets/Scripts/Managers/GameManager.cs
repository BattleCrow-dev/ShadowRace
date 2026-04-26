using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using YG;

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
    [SerializeField] private List<Sprite> commonCarVariants;
    [SerializeField] private List<Sprite> fastCarVariants;
    [SerializeField] private List<Sprite> pickupCarVariants;

    private UIManager uiManager;
    private GhostsManager ghostsManager;
    private CameraManager cameraManager;
    private InputManager inputManager;

    private float lapTimer;

    private int totalCheckpoints;
    private int passedCheckpoints;

    private int currentTrack;
    private int currentCar;
    private int currentColor;
    private bool lapStarted;
    private bool isGameStarted;

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

        if (YG2.player.auth)
            ghostsManager.StartGame(currentTrack, currentCar, currentColor);

        AudioManager.instance.StartEngineSounds(currentCar);
    }

    private void Update()
    {
        if (lapStarted && isGameStarted)
        {
            lapTimer += Time.deltaTime;
            uiManager.UpdateLapTimer(lapTimer);          
        }

        if (isGameStarted && Keyboard.current.escapeKey.wasPressedThisFrame)
            ReloadScene();
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

            if (YG2.player.auth)
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
        List<float> times = ghostsManager.GetGhostsTimes();
        times.Add(lapTimer);

        List<string> names = ghostsManager.GetGhostsNames();

        if (!YG2.player.auth)
            names.Add("");
        else
            names.Add($"> {YG2.player.name} <");

        List<Sprite> cars = ghostsManager.GetGhostsSprites();
        switch(currentCar)
        {
            case 0:
                cars.Add(commonCarVariants[currentColor]);
                break;
            case 1:
                cars.Add(fastCarVariants[currentColor]);
                break;
            case 2:
                cars.Add(pickupCarVariants[currentColor]);
                break;
            default:
                cars.Add(commonCarVariants[currentColor]);
                break;
        }

        var indices = Enumerable.Range(0, times.Count).ToArray();
        Array.Sort(indices, (a, b) => times[a].CompareTo(times[b]));

        var tempTimes = times.ToArray();
        var tempNames = names.ToArray();
        var tempCars = cars.ToArray();

        for (int i = 0; i < indices.Length; i++)
        {
            times[i] = tempTimes[indices[i]];
            names[i] = tempNames[indices[i]];
            cars[i] = tempCars[indices[i]];
        }

        if (YG2.player.auth)
            ghostsManager.FinishLap();

        lapTimer = 0f;
        passedCheckpoints = 0;
        isGameStarted = false;
        uiManager.ShowFinishPanel(times, names, cars);
        SavesManager.Instance.SaveData();
    }

    public void ReloadScene()
    {
        AudioManager.instance.ChangeMusic(-1);
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

    public void ChooseCar(int carIndex, int colorIndex)
    {
        commonCar.SetActive(false);
        fastCar.SetActive(false);
        pickupCar.SetActive(false);
        currentCar = carIndex;
        currentColor = colorIndex;

        switch (carIndex)
        {
            case 0:
                commonCar.SetActive(true);
                commonCar.GetComponent<SpriteRenderer>().sprite = commonCarVariants[colorIndex];
                cameraManager.SetTarget(commonCar.transform);
                ghostsManager.SetPlayerCar(commonCar.transform);
                break;
            case 1:
                fastCar.SetActive(true);
                fastCar.GetComponent<SpriteRenderer>().sprite = fastCarVariants[colorIndex];
                cameraManager.SetTarget(fastCar.transform);
                ghostsManager.SetPlayerCar(fastCar.transform);
                break;
            case 2:
                pickupCar.SetActive(true);
                pickupCar.GetComponent<SpriteRenderer>().sprite = pickupCarVariants[colorIndex];
                cameraManager.SetTarget(pickupCar.transform);
                ghostsManager.SetPlayerCar(pickupCar.transform);
                break;
            default:
                commonCar.SetActive(true);
                commonCar.GetComponent<SpriteRenderer>().sprite = commonCarVariants[colorIndex];
                cameraManager.SetTarget(commonCar.transform);
                ghostsManager.SetPlayerCar(commonCar.transform);
                break;
        }
    }

    public int GetCurrentTrack() => currentTrack;
    public bool GetIsGameStarted() => isGameStarted;
}
