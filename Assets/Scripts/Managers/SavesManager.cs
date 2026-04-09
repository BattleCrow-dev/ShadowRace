using System.Collections.Generic;
using UnityEngine;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int curTrack = 0;
        public int curCar = 0;
        public int curCarColor = 0;

        public float curMusicVolume = 0.7f;
        public float curSoundsVolume = 1f;

        public List<float> bestResults = new() { -1f, -1f, -1f };
    }
}

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public float GetBestResult(int trackIndex)
    {   
        return YG2.saves.bestResults[trackIndex]; 
    }

    public void SetBestResult(int trackIndex, float value) 
    { 
        if (GetBestResult(trackIndex) > value || GetBestResult(trackIndex) == -1)
            YG2.saves.bestResults[trackIndex] = value;
        SaveData();
    }

    public int GetCurTrack() => YG2.saves.curTrack;
    public void SetCurTrack(int index) => YG2.saves.curTrack = index;
    public int GetCurCar() => YG2.saves.curCar;
    public void SetCurCar(int index) => YG2.saves.curCar = index;
    public int GetCurCarColor() => YG2.saves.curCarColor;
    public void SetCurCarColor(int index) => YG2.saves.curCarColor = index;
    public float GetCurMusicVolume() => YG2.saves.curMusicVolume;
    public void SetCurMusicVolume(float value) => YG2.saves.curMusicVolume = value;
    public float GetCurSoundsVolume() => YG2.saves.curSoundsVolume;
    public void SetCurSoundsVolume(float value) => YG2.saves.curSoundsVolume = value;

    public void SaveData() => YG2.SaveProgress();
}
