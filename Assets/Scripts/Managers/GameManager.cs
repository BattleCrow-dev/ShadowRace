using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;
    private GhostsManager ghostsManager;

    private float lapTimer;

    private int totalCheckpoints;
    private int passedCheckpoints;

    private bool lapStarted;

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        ghostsManager = FindAnyObjectByType<GhostsManager>();

        uiManager.UpdateLapTimer(lapTimer);
    }

    private void Update()
    {
        if (lapStarted)
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

        if (!lapStarted)
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
        ghostsManager.StartLap();
    }
}
