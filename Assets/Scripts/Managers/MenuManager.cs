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
    public class SpriteList { public List<Sprite> variants; }

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
    [SerializeField] private GameObject playButtonBack;
    [SerializeField] private GameObject[] tutorialPanels;

    [Header("Texts")]
    [SerializeField] private List<TMP_Text> bestTimeTexts;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button authPanelButton;
    [SerializeField] private Button authSettingsButton;
    [SerializeField] private Button authGuestButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button garageBackButton;
    [SerializeField] private Button tracksButton;
    [SerializeField] private Button tracksBackButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private Button rewardsBackButton;
    [SerializeField] private List<Button> tracksChooseButtons;
    [SerializeField] private List<Button> leaderboardsChooseButtons;
    [SerializeField] private List<Button> carsChooseButtons;
    [SerializeField] private List<Button> carsColorsChooseButtons;

    [Header("Leaderboards")]
    [SerializeField] private List<GameObject> leaderboardsPanels;

    [Header("Sprites")]
    [SerializeField] private List<SpriteList> carsColorVariants;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundsVolumeSlider;

    private int curCarIndex, curCarColorIndex;
    private int curTutorialIndex;
    private static bool isGuestMode = false;

    private void Start()
    {
        authPanelButton.onClick.AddListener(StartAuth);
        authSettingsButton.onClick.AddListener(StartAuth);
        authGuestButton.onClick.AddListener(SkipAuth);
        playButton.onClick.AddListener(StartGame);

        garageButton.onClick.AddListener(OpenGarage);
        garageBackButton.onClick.AddListener(CloseGarage);
        tracksButton.onClick.AddListener(OpenTracks);
        tracksBackButton.onClick.AddListener(CloseTracks);
        settingsButton.onClick.AddListener(OpenSettings);
        settingsBackButton.onClick.AddListener(CloseSettings);
        rewardsButton.onClick.AddListener(OpenRewards);
        rewardsBackButton.onClick.AddListener(CloseRewards);

        if (YG2.player.auth)
            OnAuthorized();
        else
        {
            StartCoroutine(nameof(AuthChecking));
            if (!isGuestMode)
                authPanel.SetActive(true);
            else
                SkipAuth();
        }
    }

    private void StartAuth()
    {
        YG2.OpenAuthDialog();
    }

    private void SkipAuth()
    {
        isGuestMode = true;
        authPanel.SetActive(false);
        authSettingsButton.gameObject.SetActive(true);
        rewardsButton.GetComponent<Animation>().enabled = false;
        tracksButton.GetComponent<Animation>().enabled = true;
        garageButton.GetComponent<Animation>().enabled = true;
        playButton.GetComponent<Animation>().enabled = true;
        settingsButton.GetComponent<Animation>().enabled = true;
        playButtonBack.GetComponent<Animation>().enabled = true;
        rewardsButton.interactable = false;
        LoadSaves();
    }

    private void OnAuthorized()
    {
        isGuestMode = false;
        authSettingsButton.gameObject.SetActive(false);

        authPanel.SetActive(false);
        mainPanel.SetActive(true);

        tracksButton.GetComponent<Animation>().enabled = true;
        garageButton.GetComponent<Animation>().enabled = true;
        rewardsButton.GetComponent<Animation>().enabled = true;
        playButton.GetComponent<Animation>().enabled = true;
        settingsButton.GetComponent<Animation>().enabled = true;
        playButtonBack.GetComponent<Animation>().enabled = true;

        rewardsButton.interactable = true;

        LoadSaves();
    }

    private void LoadSaves()
    {
        for (int i = 0; i < bestTimeTexts.Count; i++)
        {
            if (SavesManager.Instance.GetBestResult(i) != -1f)
            {
                if (YG2.lang == "ru")
                    bestTimeTexts[i].text = $"Лучший результат:\n{string.Format("{0:f2}", SavesManager.Instance.GetBestResult(i))} сек.";
                else
                    bestTimeTexts[i].text = $"Best result:\n{string.Format("{0:f2}", SavesManager.Instance.GetBestResult(i))} sec.";
            }
            else
            {
                if (YG2.lang == "ru")
                    bestTimeTexts[i].text = $"Лучший результат:\nне установлен";
                else
                    bestTimeTexts[i].text = $"Best result:\nnot been set";
            }
        }

        ChooseTrack(SavesManager.Instance.GetCurTrack());
        ChooseCar(SavesManager.Instance.GetCurCar());
        ChooseCarColor(SavesManager.Instance.GetCurCarColor());

        AudioManager.instance.SetMusicVolume(SavesManager.Instance.GetCurMusicVolume() / musicVolumeSlider.maxValue);
        AudioManager.instance.SetSoundsVolume(SavesManager.Instance.GetCurSoundsVolume() / soundsVolumeSlider.maxValue);

        if (!SavesManager.Instance.GetWatchedTutorial())
        {
            if (!isGuestMode)
                CloseSettings();

            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        curTutorialIndex = 0;

        tracksButton.GetComponent<Animation>().enabled = false;
        garageButton.GetComponent<Animation>().enabled = false;
        rewardsButton.GetComponent<Animation>().enabled = false;
        playButton.GetComponent<Animation>().enabled = false;
        settingsButton.GetComponent<Animation>().enabled = false;
        playButtonBack.GetComponent<Animation>().enabled = false;

        tutorialPanels[curTutorialIndex].SetActive(true);
    }

    public void NextTutorial()
    {
        tutorialPanels[curTutorialIndex].SetActive(false);
        curTutorialIndex++;

        if (curTutorialIndex < tutorialPanels.Length)
            tutorialPanels[curTutorialIndex].SetActive(true);

        if (curTutorialIndex == tutorialPanels.Length)
        {
            tracksButton.GetComponent<Animation>().enabled = true;
            garageButton.GetComponent<Animation>().enabled = true;
            if (!isGuestMode)
                rewardsButton.GetComponent<Animation>().enabled = true;
            playButton.GetComponent<Animation>().enabled = true;
            settingsButton.GetComponent<Animation>().enabled = true;
            playButtonBack.GetComponent<Animation>().enabled = true;

            SavesManager.Instance.SetWatchedTutorial(true);
            SavesManager.Instance.SaveData();
        }
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
        ChooseLeaderboard(0);
        rewardsPanel.GetComponent<Animation>().Play("RewardsPanel");
    }

    private void CloseRewards()
    {
        rewardsPanel.GetComponent<Animation>().Play("RewardsPanel_Close");
    }

    public void ChooseLeaderboard(int index)
    {
        for (int i = 0; i < leaderboardsChooseButtons.Count; i++)
        {
            if (i == index)
            {
                leaderboardsChooseButtons[i].interactable = false;
                leaderboardsPanels[i].SetActive(true);
                leaderboardsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.05f, 1.05f, 1.05f);
            }
            else
            {
                leaderboardsChooseButtons[i].interactable = true;
                leaderboardsPanels[i].SetActive(false);
                leaderboardsChooseButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }
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

    private IEnumerator AuthChecking()
    {
        while (!YG2.player.auth)       
            yield return new WaitForSeconds(0.1f);

        OnAuthorized();
    }
}
