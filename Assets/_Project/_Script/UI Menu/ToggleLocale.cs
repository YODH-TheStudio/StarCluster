using System.Collections.Generic;
using MeetAndTalk.Localization;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UnityEngine.Localization.Samples
{
    public class ToggleLocale : MonoBehaviour
    {
        public TextMeshProUGUI  languageText;
        private int _currentLocaleIndex = 0;

        AsyncOperationHandle m_InitializeOperation;
        Dictionary<Locale, Toggle> m_Toggles = new Dictionary<Locale, Toggle>();
        ToggleGroup m_ToggleGroup;

        void Start()
        {
            // SelectedLocaleAsync will ensure that the locales have been initialized and a locale has been selected.
            m_InitializeOperation = LocalizationSettings.SelectedLocaleAsync;
            if (m_InitializeOperation.IsDone)
            {
                InitializeCompleted(m_InitializeOperation);
            }
            else
            {
                m_InitializeOperation.Completed += InitializeCompleted;
            }
            
            // Set current locale
            _currentLocaleIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            
            // set button text
            languageText.text = LocalizationSettings.SelectedLocale.Identifier.CultureInfo != null
                ? LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName switch
                {
                    "fr" => "Français",
                    "en" => "English",
                    _ => LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName
                }
                : LocalizationSettings.SelectedLocale.ToString();
        }

        void InitializeCompleted(AsyncOperationHandle obj) 
        {
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        }

        public void NextLanguage()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (_currentLocaleIndex == locales.Count - 1)
            {
                _currentLocaleIndex = 0;
            }
            else
            {
                _currentLocaleIndex++;
            }
            // Unsubscribe from SelectedLocaleChanged so we don't get an unnecessary callback from the change we are about to make.
            LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;

            LocalizationSettings.SelectedLocale = locales[_currentLocaleIndex];
            
            // set button text
            languageText.text = LocalizationSettings.SelectedLocale.Identifier.CultureInfo != null
                ? LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName switch
                {
                    "fr" => "Français",
                    "en" => "English",
                    _ => LocalizationSettings.SelectedLocale.Identifier.CultureInfo.NativeName
                }
                : LocalizationSettings.SelectedLocale.ToString();

            // Resubscribe to SelectedLocaleChanged so that we can stay in sync with changes that may be made by other scripts.
            LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
            
            LocalizationManager lm = Resources.Load("Languages") as LocalizationManager;
        
            if (LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName == "en")
            {
                lm.selectedLang = SystemLanguage.English;
            }
            else if (LocalizationSettings.SelectedLocale.Identifier.CultureInfo.TwoLetterISOLanguageName == "fr")
            {
                lm.selectedLang = SystemLanguage.French;
            }
        }

        void LocalizationSettings_SelectedLocaleChanged(Locale locale)
        {
            if (m_Toggles.TryGetValue(locale, out var toggle))
            {
                toggle.SetIsOnWithoutNotify(true);
            }
        }
    }
}
