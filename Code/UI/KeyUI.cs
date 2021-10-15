// Primary Author : Maximiliam Rosén - maka4519

using Entity.Player;
using Environment.Door;
using Environment.RoomManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class KeyUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text = default;
        [SerializeField]
        private Image icon = default;

        private int _currentAmount;
        private int _totalAmount;

        private void Awake()
        {
            PlayerController.OnPlayerDied += Deactivate;
            Door.onKeyOpenDoor += Deactivate;
            Room.OnInitKey += InitKeyUI;
            Room.OnPickedKey += Increase;
        }

        private void OnDestroy()
        {
            PlayerController.OnPlayerDied -= Deactivate;
            Door.onKeyOpenDoor -= Deactivate;
            Room.OnInitKey -= InitKeyUI;
            Room.OnPickedKey -= Increase;
        }

        private void InitKeyUI(int totalKeys)
        {
            _totalAmount = totalKeys;
            Activate();
        }

        private void Activate()
        {
            _currentAmount = 0;
            icon.gameObject.SetActive(true);
            text.gameObject.SetActive(true);
            UpdateUIText();
        }

        private void Deactivate()
        {
            ClearText();
            icon.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }

        private void ClearText()
        {
            text.text = "";
        }

        private void Increase()
        {
            _currentAmount += 1;
            UpdateUIText();
        }

        private void UpdateUIText()
        {
            text.text = $"{_currentAmount}/{_totalAmount}";
        }
    }
}