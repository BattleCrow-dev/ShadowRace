using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public int[] openedSkins = { 0, 0, 0 };
        public int[] openedTracks = { 0, 0, 0, 0 };
        public int curSkinIndex = 0;
        public int curColorIndex = 0;
    }
}

public class SavesManager : MonoBehaviour
{

    public bool OpenSkin(int index)
    {
        if (!(index > YG2.saves.openedSkins.Length) && YG2.saves.openedSkins[index] == 0)
        {
            YG2.saves.openedSkins[index] = 1;
            return true;
        }

        return false;
    }

    public bool OpenTrack(int index) 
    {
        if (!(index > YG2.saves.openedTracks.Length) && YG2.saves.openedTracks[index] == 0)
        { 
            YG2.saves.openedTracks[index] = 1; 
            return true; 
        }

        return false;
    }

    public void SetCurSkinIndex(int index) => YG2.saves.curSkinIndex = index;
    public void SetCurColorIndex(int index) => YG2.saves.curColorIndex = index;
}
