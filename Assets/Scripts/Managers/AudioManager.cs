using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Elements")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundsSource;
    [SerializeField] private AudioSource motorSource;

    [Header("Musics")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip firstTrackMusic;
    [SerializeField] private AudioClip secondTrackMusic;
    [SerializeField] private AudioClip thirdTrackMusic;

    [Header("Sounds")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip sliderSound;
    [SerializeField] private AudioClip motorSound;

    private float currentMinPitch;
    private float currentMaxPitch;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeMusic(int trackIndex)
    {
        switch (trackIndex)
        {
            case -1:
                StartCoroutine(MusicChanging(menuMusic));
                break;
            case 0:
                StartCoroutine(MusicChanging(firstTrackMusic));
                break;
            case 1:
                StartCoroutine(MusicChanging(secondTrackMusic));
                break;
            case 2:
                StartCoroutine(MusicChanging(thirdTrackMusic));
                break;
            default:
                StartCoroutine(MusicChanging(menuMusic));
                break;
        }
    }

    private IEnumerator MusicChanging(AudioClip newMusic)
    {
        float startVolume = musicSource.volume;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        musicSource.volume = 0f;

        musicSource.clip = newMusic;
        musicSource.Play();

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, elapsedTime / duration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    public void StartEngineSounds(int carType)
    {
        SetupEngine(carType);
        motorSource.clip = motorSound;
        motorSource.loop = true;
        motorSource.Play();
    }

    public void StopEngineSounds()
    {
        motorSource.loop = false;
    }

    public void SetupEngine(int carType)
    {
        switch (carType)
        {
            case 0:
                currentMinPitch = 0.9f;
                currentMaxPitch = 1.4f;
                break;
            case 1:
                currentMinPitch = 1.1f;
                currentMaxPitch = 1.9f;
                break;
            case 2:
                currentMinPitch = 0.7f;
                currentMaxPitch = 1.2f;
                break;
        }
    }

    public void UpdateEngineSound(bool isAccelerating)
    {
        motorSource.pitch = Mathf.Clamp(
            motorSource.pitch + (isAccelerating ? 0.05f : -0.05f),
            currentMinPitch,
            currentMaxPitch
        );
    }

    public void PlayClickSound() => soundsSource.PlayOneShot(clickSound);
    public void PlaySliderSound() => soundsSource.PlayOneShot(sliderSound);

    public void SetMusicVolume(float value) => musicSource.volume = value;
    public void SetSoundsVolume(float value)
    {
        soundsSource.volume = value;
        motorSource.volume = value;
    }
}