// Primary Author : Maximiliam Rosén - maka4519

using TMPro;
using UnityEngine;

namespace UI
{
    public class TextBackground : MonoBehaviour
    {
        private TextMeshProUGUI _background;

        private void Start()
        {
            _background = GetComponent<TextMeshProUGUI>();
            SubtitleManager.onTextChanged += UpdateBackground;
            SubtitleManager.onHide += ONHide;
            SubtitleManager.onShow += ONShow;
        }

        private void ONShow()
        {
            _background.enabled = true;
        }

        private void ONHide()
        {
            _background.enabled = false;
        }

        private void UpdateBackground(string text)
        {
            if (text == "")
            {
                _background.SetText("");
            }

            _background.SetText(@"<mark=#000000CC padding=""50, 50, 5, 0"">" + text + "</mark>");
        }
    }
}