using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using YG;

public class MainController : MonoBehaviour
{
    public TMP_Text currentLapText;
    public TMP_Text bestLapText;

    public GhostSystem ghostSystem;

    private float lapTimer;
    private float bestLapTime;

    private int totalCheckpoints;
    private int passedCheckpoints;

    private bool lapStarted;

    private string bestLapKey;

    private void Start()
    {
        bestLapKey = "BEST_LAP_" + SceneManager.GetActiveScene().name;
        bestLapTime = PlayerPrefs.GetFloat(bestLapKey, 0f);
        UpdateUI();
    }

    private void Update()
    {
        if (lapStarted)
        {
            lapTimer += Time.deltaTime;
            UpdateUI();
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
            ghostSystem.StartLap();
            return;
        }
        if (passedCheckpoints == totalCheckpoints)
        {
            CompleteLap();
        }
    }

    public void CompleteLap()
    {
        if (bestLapTime == 0f || lapTimer < bestLapTime)
        {
            bestLapTime = lapTimer;

            PlayerPrefs.SetFloat(bestLapKey, bestLapTime);
            PlayerPrefs.Save();
        }

        lapTimer = 0f;
        passedCheckpoints = 0;
        UpdateUI();

        ghostSystem.FinishLap();
        ghostSystem.StartLap();
    }

    private void UpdateUI()
    {
        currentLapText.text = $"Текущее время: {FormatTime(lapTimer)}";

        if (bestLapTime > 0f)
            bestLapText.text = $"Лучшее время: {FormatTime(bestLapTime)}";
        else
            bestLapText.text = "Лучшее время: --:--:---";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}