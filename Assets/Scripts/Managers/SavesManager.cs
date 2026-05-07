using System.Collections.Generic;
using UnityEngine;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int track = 0;
        public int car = 0;
        public int color = 0;

        public float musicVolume = 7f;
        public float soundsVolume = 5f;

        public bool watchedTutorial = false;

        public List<float> bestTimes = new() { -1f, -1f, -1f };
    }
}

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;

    [Header("Parameters")]
    [SerializeField] private string[] leaderboardsNames;

    private void Awake()
    {
        Instance = this;
    }

    public float GetBestResult(int trackIndex)
    {   
        return YG2.saves.bestTimes[trackIndex]; 
    }

    public void SetBestResult(int trackIndex, int carIndex, int colorIndex, float value)
    {
        if (GetBestResult(trackIndex) > value || GetBestResult(trackIndex) == -1)
        { 
            YG2.saves.bestTimes[trackIndex] = value;
            YG2.SetLBTimeConvert(leaderboardsNames[trackIndex], value, $"{carIndex} {colorIndex}");
        }

        SaveData();
    }

    public int GetCurTrack() => YG2.saves.track;
    public void SetCurTrack(int index) => YG2.saves.track = index;
    public int GetCurCar() => YG2.saves.car;
    public void SetCurCar(int index) => YG2.saves.car = index;
    public int GetCurCarColor() => YG2.saves.color;
    public void SetCurCarColor(int index) => YG2.saves.color = index;
    public float GetCurMusicVolume() => YG2.saves.musicVolume;
    public void SetCurMusicVolume(float value) => YG2.saves.musicVolume = value;
    public float GetCurSoundsVolume() => YG2.saves.soundsVolume;
    public void SetCurSoundsVolume(float value) => YG2.saves.soundsVolume = value;
    public bool GetWatchedTutorial() => YG2.saves.watchedTutorial;
    public void SetWatchedTutorial(bool value) => YG2.saves.watchedTutorial = value;
    public void SaveData() => YG2.SaveProgress();
}
