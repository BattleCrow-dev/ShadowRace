using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI_Panels")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private GameObject infoPanel;

    [Header("UI_Texts")]
    [SerializeField] private TMP_Text currentLapText;
    [SerializeField] private TMP_Text[] finishNames;
    [SerializeField] private TMP_Text[] finishTimes;

    [Header("UI_Images")]
    [SerializeField] private Image[] finishCarsImages;

    [Header("Mobile_UI_Elements")]
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject mobileMovementHintText;

    [Header("PC_UI_Elements")]
    [SerializeField] private GameObject PcMovementHintText;

    public void ShowMobileUI()
    {
        joystick.SetActive(true);
        mobileMovementHintText.SetActive(true);
        PcMovementHintText.SetActive(false);
    }

    public void ShowPCUI()
    {
        joystick.SetActive(false);
        mobileMovementHintText.SetActive(false);
        PcMovementHintText.SetActive(true);
    }

    public void ShowFinishPanel(List<float> times, List<string> names, List<Sprite> cars)
    {
        for(int i = 0; i < 3; i++)
        {
            finishTimes[i].text = string.Format("{0:f2}", times[i]);
            finishNames[i].text = names[i];
            finishCarsImages[i].sprite = cars[i];
        }

        finishPanel.SetActive(true);
        infoPanel.SetActive(false);
        joystick.SetActive(false);
    }

    public void UpdateLapTimer(float time)
    {
        currentLapText.text = $"Текущее время: {FormatTime(time)}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
