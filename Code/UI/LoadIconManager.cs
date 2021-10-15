// Primary Author : Viktor Dahlberg - vida6631

using Framework.ScriptableObjectVariables.Custom;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadIconManager : MonoBehaviour
    {
        [SerializeField]
        private ScriptObjBoolGroup busy = default;
        [SerializeField]
        private Image loadingImage = default;
        [SerializeField]
        private float rotationSpeed = default;

        private void Update()
        {
            loadingImage.enabled = !busy.IsAllTrue();
            if (loadingImage.enabled) loadingImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}