// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Camera
{
    /// <summary>
    /// Manages cursor lock state.
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        private static bool _automatic = true;

        public static void ToggleAutomatic(bool isAutomatic)
        {
            _automatic = isAutomatic;
        }

        public static void Lock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (_automatic && Mouse.current.leftButton.isPressed && Cursor.lockState == CursorLockMode.None)
            {
                Lock();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (_automatic)
            {
                if (focus)
                {
                    Lock();
                }
                else
                {
                    Unlock();
                }
            }
        }
    }
}