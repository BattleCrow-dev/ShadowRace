using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int curTrack = 0;
        public int curCar = 0;
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

        YG2.SaveProgress();
    }

    public int GetCurTrack() => YG2.saves.curTrack;
    public void SetCurTrack(int index) { YG2.saves.curTrack = index; YG2.SaveProgress(); }
    public int GetCurCar() => YG2.saves.curCar;
    public void SetCurCar(int index) { YG2.saves.curCar = index; YG2.SaveProgress(); }

    }
