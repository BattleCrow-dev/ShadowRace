using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if TMP_YG2
using TMPro;
#endif

namespace YG
{
    public class LBPlayerDataYG : MonoBehaviour
    {
        public ImageLoadYG imageLoad;
        public Image carImage;

        [Serializable]
        public struct TextLegasy
        {
            public Text rank, name, score;
        }
        public TextLegasy textLegasy;

#if TMP_YG2
        [Serializable]
        public struct TextMP
        {
            public TextMeshProUGUI rank, name, score;
        }
        public TextMP textMP;
#endif
        [Space(10)]
        public MonoBehaviour[] topPlayerActivityComponents = new MonoBehaviour[0];
        public MonoBehaviour[] currentPlayerActivityComponents = new MonoBehaviour[0];

        [Space(10)]
        public List<MenuManager.SpriteList> carsColorVariants;

        public class Data
        {
            public string rank;
            public string name;
            public string score;
            public int carIndex;
            public int colorIndex;
            public string photoUrl;
            public bool inTop;
            public int topIndex;
            public bool currentPlayer;
            public Sprite photoSprite;
        }

        [HideInInspector]
        public Data data = new Data();

        public void UpdateEntries()
        {
            if (textLegasy.rank && data.rank != null) textLegasy.rank.text = data.rank.ToString();
            if (textLegasy.name && data.name != null) textLegasy.name.text = data.name;
            if (textLegasy.score && data.score != null) textLegasy.score.text = data.score.ToString();

#if TMP_YG2
            if (textMP.rank && data.rank != null) textMP.rank.text = data.rank.ToString();
            if (textMP.name && data.name != null) textMP.name.text = data.name;
            if (textMP.score && data.score != null) textMP.score.text = data.score.ToString();
#endif
            if (imageLoad)
            {
                if (data.photoSprite)
                {
                    imageLoad.SetTexture(data.photoSprite.texture);
                }
                else if (data.photoUrl == null)
                {
                    imageLoad.ClearTexture();
                }
                else
                {
                    imageLoad.Load(data.photoUrl);
                }
            }

            if (carImage)
            {
                carImage.sprite = carsColorVariants[data.carIndex].variants[data.colorIndex];
            }

            if (topPlayerActivityComponents.Length > 0)
            {
                if (data.inTop)
                {
                    ActivityMomoObjects(topPlayerActivityComponents, true, data.topIndex);
                }
                else
                {
                    ActivityMomoObjects(topPlayerActivityComponents, false, data.topIndex);
                }
            }

            if (currentPlayerActivityComponents.Length > 0)
            {
                if (data.currentPlayer)
                {
                    ActivityMomoObjects(currentPlayerActivityComponents, true, 0);
                }
                else
                {
                    ActivityMomoObjects(currentPlayerActivityComponents, false, 0);
                }
            }

            void ActivityMomoObjects(MonoBehaviour[] objects, bool activity, int index)
            {
                objects[index].enabled = activity;
            }
        }
    }
}