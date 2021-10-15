// Primary Author : Viktor Dahlberg - vida6631

using Framework.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameSaveEvaluator : MonoBehaviour
    {
        [SerializeField]
        private Button button = default;
        [SerializeField]
        private TextMeshProUGUI buttonText = default;
        [SerializeField]
        private Color enabledTextColor = default;
        [SerializeField]
        private Color disabledTextColor = default;

        private void OnEnable()
        {
            button.interactable = SaveManager.HasSave();
            buttonText.color = button.interactable ? enabledTextColor : disabledTextColor;
        }
    }
}