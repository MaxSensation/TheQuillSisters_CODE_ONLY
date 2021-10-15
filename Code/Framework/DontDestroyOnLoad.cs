// Primary Author : Andreas Berzelius - anbe5918

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framemwork
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField]
        private ScriptObjVar<bool> alreadyDontDestroyOnLoad = default;

        private void Start()
        {
            if (alreadyDontDestroyOnLoad == false)
            {
                alreadyDontDestroyOnLoad.value = true;
                DontDestroyOnLoad(this);
            }
        }

        private void OnApplicationQuit()
        {
            alreadyDontDestroyOnLoad.value = false;
        }
    }
}