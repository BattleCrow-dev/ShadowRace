using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;
    private GhostsManager ghostsManager;
    private InputManager inputManager;

    private float lapTimer;

    private int totalCheckpoints;
    private int passedCheckpoints;

    private bool lapStarted;
    private bool isGameStarted;

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        ghostsManager = FindAnyObjectByType<GhostsManager>();
        inputManager = FindAnyObjectByType<InputManager>();

        uiManager.UpdateLapTimer(lapTimer);
    }

    public void StartGame()
    {
        isGameStarted = true;
        inputManager.ShowUI();
        ghostsManager.StartGame();
    }

    private void Update()
    {
        if (lapStarted && isGameStarted)
        {
            lapTimer += Time.deltaTime;
            uiManager.UpdateLapTimer(lapTimer);
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

    public bool GetIsGameStarted() => isGameStarted;
}
