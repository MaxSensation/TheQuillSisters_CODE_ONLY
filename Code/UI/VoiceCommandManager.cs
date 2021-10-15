// Primary Author : Andreas Berzelius - anbe5918

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

namespace UI
{
    /// <summary>
    ///     A simple voiceCommand script for short commands and navigating the
    ///     main menu.
    /// </summary>
    public class VoiceCommandManager : MonoBehaviour
    {
        [SerializeField]
        private Image micOffImage = default;
        [SerializeField]
        private GameObject voiceInstructions = default;
        
        [Header("Shortcut Command Buttons")] 
        
        [SerializeField]
        private Button startButton = default;
        [SerializeField]
        private Button quitButton = default;
        [SerializeField]
        private Button settingsButton = default;

        public static Action Back;
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        private bool _active;
        private Button _currentSelectedButton;
        private KeywordRecognizer _keywordRecognizer;

        private void Start()
        {
            _actions.Add("Select", Select);
            _actions.Add("Go", Select);
            _actions.Add("up", Up);
            _actions.Add("down", Down);
            _actions.Add("back", GoBack);
            _actions.Add("Start", StartGame);
            _actions.Add("NewGame", StartGame);
            _actions.Add("Quit", QuitGame);
            _actions.Add("Setting", OpenSettings);
            _actions.Add("Options", OpenSettings);
            EventSystem.current.firstSelectedGameObject.TryGetComponent(out _currentSelectedButton);
            _keywordRecognizer = new KeywordRecognizer(_actions.Keys.ToArray());
            _keywordRecognizer.OnPhraseRecognized += RecognizedCommand;
        }

        public void OnDisable()
        {
            if (_keywordRecognizer.IsRunning)
            {
                _keywordRecognizer.Stop();
            }

            _active = false;
            micOffImage.enabled = true;
            voiceInstructions.SetActive(false);
        }

        private void GoBack()
        {
            Back?.Invoke();
            GetNewSelected();
        }

        private void Select()
        {
            _currentSelectedButton.onClick.Invoke();
            GetNewSelected();
        }

        private void GetNewSelected()
        {
            EventSystem.current.currentSelectedGameObject.TryGetComponent(out _currentSelectedButton);
            voiceInstructions.SetActive(false);
        }

        private void Down()
        {
            _currentSelectedButton.navigation.selectOnDown.Select();
            GetNewSelected();
        }

        private void Up()
        {
            _currentSelectedButton.navigation.selectOnUp.Select();
            GetNewSelected();
        }

        //Shortcut command Code
        private void StartGame()
        {
            startButton.onClick.Invoke();
        }

        private void QuitGame()
        {
            quitButton.onClick.Invoke();
        }

        private void OpenSettings()
        {
            settingsButton.onClick.Invoke();
            GetNewSelected();
        }

        ///
        public void Pressed()
        {
            if (!_active)
            {
                _keywordRecognizer.Start();
                _active = true;
                micOffImage.enabled = false;
                voiceInstructions.SetActive(true);
            }
            else if (_active)
            {
                _keywordRecognizer.Stop();
                _active = false;
                micOffImage.enabled = true;
                voiceInstructions.SetActive(false);
            }
        }

        private void RecognizedCommand(PhraseRecognizedEventArgs command)
        {
            _actions[command.text].Invoke();
        }
    }
}