using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MenuManager : MonoBehaviour
{
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

    [Header("Texts")]
    [SerializeField] private List<TMP_Text> bestTimeTexts;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button authButton;
    [SerializeField] private Button garageButton;
    [SerializeField] private Button garageBackButton;
    [SerializeField] private Button tracksButton;
    [SerializeField] private Button tracksBackButton;
    [SerializeField] private List<Button> tracksChooseButtons;
    [SerializeField] private List<Button> carsChooseButtons;

    private void Start()
    {
        authButton.onClick.AddListener(StartAuth);
        playButton.onClick.AddListener(StartGame);

        garageButton.onClick.AddListener(OpenGarage);
        garageBackButton.onClick.AddListener(CloseGarage);
        tracksButton.onClick.AddListener(OpenTracks);
        tracksBackButton.onClick.AddListener(CloseTracks);

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
    }

    private void StartGame()
    {
        StartCoroutine(nameof(CameraZoom));
    }

    private void OpenGarage()
    {
        garagePanel.GetComponent<Animation>().Play("GaragePanel");
    }

    private void CloseGarage()
    {
        garagePanel.GetComponent<Animation>().Play("GaragePanel_Close");
    }

    private void OpenTracks()
    {
        tracksPanel.GetComponent<Animation>().Play("TrackPanel");
    }

    private void CloseTracks()
    {
        tracksPanel.GetComponent<Animation>().Play("TrackPanel_Close");
    }

    public void ChooseTrack(int index)
    {
        gameManager.ChooseTrack(index);
        SavesManager.Instance.SetCurTrack(index);

        for (int i = 0; i < tracksChooseButtons.Count; i++)
            tracksChooseButtons[i].interactable = !(i == index);
    }

    public void ChooseCar(int index)
    {
        gameManager.ChooseCar(index);
        SavesManager.Instance.SetCurCar(index);

        for (int i = 0; i < carsChooseButtons.Count; i++)
            carsChooseButtons[i].interactable = !(i == index);
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
