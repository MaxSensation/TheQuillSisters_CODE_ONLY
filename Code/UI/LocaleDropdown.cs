// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class LocaleDropdown : MonoBehaviour
    {
        public TMP_Dropdown dropdown;

        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;
            var options = new List<TMP_Dropdown.OptionData>();
            var selected = 0;
            for (var i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                {
                    selected = i;
                }

                options.Add(new TMP_Dropdown.OptionData(locale.name));
            }

            dropdown.options = options;
            dropdown.value = selected;
            dropdown.onValueChanged.AddListener(LocaleSelected);
        }

        private static void LocaleSelected(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
    }
}