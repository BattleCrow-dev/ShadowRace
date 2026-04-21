using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MenuManager : MonoBehaviour
{
    [Serializable]
    private class SpriteList { public List<Sprite> variants; }

    [Header("Elements")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera menuCamera, gameCamera;
    [SerializeField] private GameObject menuUI, gameUI;

    [Header("Panels")]
    [SerializeField] private GameObject authPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private GameObject garagePanel;
    [SerializeField] private GameObject tracksPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject rewardsPanel;

    [Header("Texts")]
    [SerializeField] private List<TMP_Text> bestTimeTexts;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button authButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button garageBackButton;
    [SerializeField] private Button tracksButton;
    [SerializeField] private Button tracksBackButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button settingsSaveButton;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private Button rewardsBackButton;
    [SerializeField] private List<Button> tracksChooseButtons;
    [SerializeField] private List<Button> carsChooseButtons;
    [SerializeField] private List<Button> carsColorsChooseButtons;

    [Header("Sprites")]
    [SerializeField] private List<SpriteList> carsColorVariants;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundsVolumeSlider;

    private int curCarIndex, curCarColorIndex;

    private void Start()
    {
        authButton.onClick.AddListener(StartAuth);
        playButton.onClick.AddListener(StartGame);

        garageButton.onClick.AddListener(OpenGarage);
        garageBackButton.onClick.AddListener(CloseGarage);
        tracksButton.onClick.AddListener(OpenTracks);
        tracksBackButton.onClick.AddListener(CloseTracks);
        settingsButton.onClick.AddListener(OpenSettings);
        settingsBackButton.onClick.AddListener(CloseSettings);
        rewardsButton.onClick.AddListener(OpenRewards);
        rewardsBackButton.onClick.AddListener(CloseRewards);

        CheckAuth();
    }

    private void CheckAuth()
    {
        if (YG2.player.auth)
            OnAuthorized();
        else
            authPanel.SetActive(true);
    }

    private void StartAuth()
    {
        YG2.OpenAuthDialog();
        OnAuthResult();
    }

    private void OnAuthResult()
    {
        if (YG2.player.auth)
            OnAuthorized();
        else
            OnUnauthorized();
    }

    private void OnAuthorized()
    {
        authPanel.SetActive(false);
        mainPanel.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            if (SavesManager.Instance.GetBestResult(i) != -1f)
                bestTimeTexts[i].text = $"Лучший результат:\n{SavesManager.Instance.GetBestResult(i)} сек.";
            else
                bestTimeTexts[i].text = $"Лучший результат: не установлен";
        }

        ChooseTrack(SavesManager.Instance.GetCurTrack());
        ChooseCar(SavesManager.Instance.GetCurCar());
        ChooseCarColor(SavesManager.Instance.GetCurCarColor());

        AudioManager.instance.SetMusicVolume(SavesManager.Instance.GetCurMusicVolume() / musicVolumeSlider.maxValue);
        AudioManager.instance.SetSoundsVolume(SavesManager.Instance.GetCurSoundsVolume() / soundsVolumeSlider.maxValue);
    }

    private void OnUnauthorized()
    {
        
    }

    private void StartGame()
    {
        StartCoroutine(nameof(CameraZoom));
        AudioManager.instance.ChangeMusic(gameManager.GetCurrentTrack());
    }

    private void OpenGarage()
    {
        garagePanel.GetComponent<Animation>().Play("GaragePanel");
    }

    private void CloseGarage()
    {
        garagePanel.GetComponent<Animation>().Play("GaragePanel_Close");
        SavesManager.Instance.SaveData();
    }

    private void OpenTracks()
    {
        tracksPanel.GetComponent<Animation>().Play("TrackPanel");
    }

    private void CloseTracks()
    {
        tracksPanel.GetComponent<Animation>().Play("TrackPanel_Close");
        SavesManager.Instance.SaveData();
    }

    private void OpenSettings()
    {
        settingsPanel.GetComponent<Animation>().Play("SettingsPanel");
        musicVolumeSlider.value = SavesManager.Instance.GetCurMusicVolume();
        soundsVolumeSlider.value = SavesManager.Instance.GetCurSoundsVolume();
    }

    private void CloseSettings()
    {
        settingsPanel.GetComponent<Animation>().Play("SettingsPanel_Close");
        SavesManager.Instance.SetCurMusicVolume(musicVolumeSlider.value);
        SavesManager.Instance.SetCurSoundsVolume(soundsVolumeSlider.value);
        SavesManager.Instance.SaveData();
    }

    private void OpenRewards()
    {
        rewardsPanel.GetComponent<Animation>().Play("RewardsPanel");
    }

    private void CloseRewards()
    {
        rewardsPanel.GetComponent<Animation>().Play("RewardsPanel_Close");
        SavesManager.Instance.SaveData();
    }

    public void ChooseTrack(int index)
    {
        gameManager.ChooseTrack(index);
        SavesManager.Instance.SetCurTrack(index);

        for (int i = 0; i < tracksChooseButtons.Count; i++)
        {
            if (i == index)
            {
                tracksChooseButtons[i].interactable = false;
                tracksChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
            }
            else
            {
                tracksChooseButtons[i].interactable = true;
                tracksChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    public void ChooseCar(int index)
    {
        gameManager.ChooseCar(index, curCarColorIndex);
        curCarIndex = index;
        SavesManager.Instance.SetCurCar(index);

        for (int i = 0; i < carsChooseButtons.Count; i++)
        {
            if (i == index)
            {
                carsChooseButtons[i].interactable = false;
                carsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
            }
            else
            {
                carsChooseButtons[i].interactable = true;
                carsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    public void ChooseCarColor(int index)
    {
        gameManager.ChooseCar(curCarIndex, index);
        curCarColorIndex = index;
        SavesManager.Instance.SetCurCarColor(index);

        for (int i = 0; i < carsColorsChooseButtons.Count; i++)
        {
            if (i == index)
            {
                carsColorsChooseButtons[i].interactable = false;
                carsColorsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.15f, 1.15f, 1.15f);
            }
            else
            {
                carsColorsChooseButtons[i].interactable = true;
                carsColorsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }    

        for (int i = 0; i < carsChooseButtons.Count; i++)
            carsChooseButtons[i].GetComponent<Image>().sprite = carsColorVariants[i].variants[index];
    }

    public void OnMusicVolumeChanges()
    {
        AudioManager.instance.SetMusicVolume(musicVolumeSlider.value / musicVolumeSlider.maxValue);
        AudioManager.instance.PlaySliderSound();
    }

    public void OnSoundsVolumeChanges()
    {
        AudioManager.instance.SetSoundsVolume(soundsVolumeSlider.value / soundsVolumeSlider.maxValue);
        AudioManager.instance.PlaySliderSound();
    }

    private IEnumerator CameraZoom()
    {
        blockPanel.SetActive(true);

        float startSize = gameCamera.orthographicSize;
        Vector3 startPosition = gameCamera.transform.position;

        float targetSize = 1.5f;
        Vector3 targetPosition = new (-100.25f, 0.6f, -10f);

        float elapsedTime = 0f;

        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.5f;

            float newSize = Mathf.Lerp(startSize, targetSize, t);
            Vector3 newPosition = Vector3.MoveTowards(startPosition, targetPosition, t);

            gameCamera.orthographicSize = newSize;
            gameCamera.transform.position = newPosition;

            yield return null;
        }

        blockPanel.SetActive(false);

        GoToGame();
    }

    private void GoToGame()
    {
        gameCamera.transform.position = menuCamera.transform.position;
        
        menuUI.SetActive(false);
        gameUI.SetActive(true);

        gameManager.StartGame();
    }
}
