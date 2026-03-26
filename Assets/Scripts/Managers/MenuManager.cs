using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button authButton;

    private void Start()
    {
        playButton.interactable = false;

        authButton.onClick.AddListener(StartAuth);
        playButton.onClick.AddListener(() => SceneManager.LoadScene(1));

        CheckAuth();
    }

    private void CheckAuth()
    {
        if (YG2.player.auth)
            OnAuthorized();
        else
            playButton.interactable = false;
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
        authButton.gameObject.SetActive(false);
        playButton.interactable = true;
    }
}
