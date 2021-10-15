// Primary Author : Andreas Berzelius - anbe5918

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class VideoSettings : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Dropdown resolutionDropDown = default;

        private Resolution[] _resolutions;

        private void Start()
        {
            _resolutions = Screen.resolutions;
            resolutionDropDown.ClearOptions();
            var options = new List<string>();
            var currentResolutionIndex = 0;
            for (var i = 0; i < _resolutions.Length; i++)
            {
                var option = _resolutions[i].width + " x " + _resolutions[i].height; 
                options.Add(option);
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropDown.AddOptions(options);
            resolutionDropDown.value = currentResolutionIndex;
            resolutionDropDown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            var resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetMaxQuality()
        {
            QualitySettings.SetQualityLevel(0);
        }

        public void SetMediumQuality()
        {
            QualitySettings.SetQualityLevel(1);
        }

        public void SetLowQuality()
        {
            QualitySettings.SetQualityLevel(2);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }
    }
}