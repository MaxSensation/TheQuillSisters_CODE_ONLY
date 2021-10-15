// Primary Author : Daterre - unity forums
// Secondary Author : Andreas Berzelius - anbe5918

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     A script to fix highlighting correctly when switching
    ///     between mouse & keyboard/gamepad.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            GetComponent<Selectable>().OnPointerExit(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
    }
}