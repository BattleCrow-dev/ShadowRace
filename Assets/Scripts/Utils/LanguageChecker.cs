using TMPro;
using UnityEngine;

namespace YG.Example
{
    public class LanguageChecker : MonoBehaviour
    {
        public string ru, en;

        private TMP_Text textComponent;

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            YG2.onSwitchLang += SwitchLanguage;
            SwitchLanguage(YG2.lang);
        }
        private void OnDisable()
        {
            YG2.onSwitchLang -= SwitchLanguage;
        }

        public void SwitchLanguage(string lang)
        {
            textComponent.text = lang switch
            {
                "ru" => ru,
                _ => en,
            };
        }
    }
}