// Primary Author : Andreas Berzelius - anbe5918

using Entity.HealthSystem;
using Framework.SceneSystem;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace UI
{
    /// <summary>
    ///     A script for enabling and disabling a Tutorialprompt.
    /// </summary>
    public class TutorialPrompt : MonoBehaviour
    {
        [SerializeField] [Tooltip("The tutorial event to listen to")]
        private GameEvent gameEvent = default;
        [SerializeField] [Tooltip("The tutorialText object to show")]
        private GameObject tutorialPrompt = default;
        [SerializeField] 
        private ScriptObjVar<bool> completed = default;

        private bool _activated;
        private GameObject _background;
        private int _seenTutorialCounter;

        private void Start()
        {
            ScenePreLoader.UnloadedActive += ResetTutorial;
            Health.EntityDied += ResetPlayerDied;
            gameEvent.OnEvent += ActivateTutorialPrompt;
            _background = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            if (tutorialPrompt.activeSelf)
            {
                _activated = true;
                _seenTutorialCounter = 1;
            }
        }

        private void ResetPlayerDied(GameObject obj)
        {
            if (obj.layer == 9 && _seenTutorialCounter < 2)
            {
                ResetTutorial();
            }
        }

        private void ResetTutorial()
        {
            _background.SetActive(false);
            tutorialPrompt.SetActive(false);
            _activated = false;
            _seenTutorialCounter = 0;
        }

        private void ActivateTutorialPrompt()
        {
            if (completed.value == false)
            {
                if (!_activated)
                {
                    _activated = true;
                    _background.SetActive(true);
                    tutorialPrompt.SetActive(true);
                    _seenTutorialCounter++;
                }
                else
                {
                    _background.SetActive(false);
                    tutorialPrompt.SetActive(false);
                    _activated = false;
                    _seenTutorialCounter++;
                }

                if (_seenTutorialCounter >= 2)
                {
                    completed.value = true;
                    _background.SetActive(false);
                    tutorialPrompt.SetActive(false);
                    _activated = false;
                }
            }
        }
    }
}