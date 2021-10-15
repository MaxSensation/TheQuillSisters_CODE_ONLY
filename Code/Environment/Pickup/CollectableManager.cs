// Primary Author : Viktor Dahlberg - vida6631

using System.Linq;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables.Sets;
using TMPro;
using UnityEngine;

namespace Environment.Pickup
{
    public class CollectableManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text = default;
        [SerializeField]
        private GameEvent saveManagerDone = default;
        [SerializeField]
        private ScriptObjScriptObjBoolSet collectables = default;

        private int _collectedAmount;
        private int _totalAmount;

        private void Awake()
        {
            foreach (var collectable in collectables.items)
            {
                collectable.ValueChanged += Refresh;
            }

            saveManagerDone.OnEvent += Refresh;
        }

        private void Refresh()
        {
            _collectedAmount = (from item in collectables.items where item.value select item).Count();
            _totalAmount = collectables.items.Count;
            text.text = $"{_collectedAmount:D2}/{_totalAmount:D2}";
        }
    }
}