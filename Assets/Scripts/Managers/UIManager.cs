using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI_Panels")]
    [SerializeField] private GameObject finishPanel;

    [Header("UI_Texts")]
    [SerializeField] private TMP_Text currentLapText;

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

    public void ShowFinishPanel()
    {
        finishPanel.SetActive(true);
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
