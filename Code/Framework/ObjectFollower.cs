// Primary Author - Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framework
{
    public class ObjectFollower : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjVar<Vector3> vector3ToFollow = default;
        [SerializeField] 
        private Vector3 offset = default;

        private void Update()
        {
            transform.position = vector3ToFollow + offset;
        }
    }
}
