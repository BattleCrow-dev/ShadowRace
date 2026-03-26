using System.Collections;
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

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button authButton;

    private void Start()
    {
        authButton.onClick.AddListener(StartAuth);
        playButton.onClick.AddListener(StartGame);

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
    }

    private void StartGame()
    {
        StartCoroutine(nameof(CameraZoom));
    }

    private IEnumerator CameraZoom()
    {
        blockPanel.SetActive(true);

        float startSize = gameCamera.orthographicSize;
        float targetSize = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.5f;

            float newSize = Mathf.Lerp(startSize, targetSize, t);
            gameCamera.orthographicSize = newSize;

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
