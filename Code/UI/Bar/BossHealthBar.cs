// Primary Author : Viktor Dahlberg - vida6631

using TMPro;
using UnityEngine;

namespace UI.Bar
{
    public class BossHealthBar : HealthBar
    {
        [Header("Boss")] 
        
        [SerializeField] 
        private TextMeshProUGUI nameField = default;
        public void SetName(string name)
        {
            nameField.text = name;
        }
    }
}