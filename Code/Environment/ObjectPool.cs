// Primary Author : Viktor Dahlberg - vida6631

using Entity.AI;
using UnityEngine;

namespace Environment
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] 
        protected GameObject[] managed = default;

        private int _startIndex;

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            Enemy.OnDied += g => RequestAt(g.transform.position + Vector3.up, Quaternion.identity);
        }

        /// <summary>
        ///     Tries to find a free pool object.
        /// </summary>
        /// <param name="position">Requested object position.</param>
        /// <param name="rotation">Requested object rotation.</param>
        /// <returns>True if an available pool object was found, False if not.</returns>
        protected virtual bool RequestAt(Vector3 position, Quaternion rotation)
        {
            var lastIteration = false;
            for (var i = _startIndex; i < managed.Length; i++)
            {
                if (!managed[i].activeInHierarchy)
                {
                    _startIndex = i;
                    Allocate(managed[i], i, position, rotation);
                    return true;
                }

                if (i == managed.Length - 1)
                {
                    if (lastIteration) break;

                    _startIndex = 0;
                    lastIteration = true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Executed when an available pool object is found.
        /// </summary>
        /// <param name="poolObject">Pool object.</param>
        /// <param name="position">Requested object position.</param>
        /// <param name="rotation">Requested object rotation.</param>
        protected virtual void Allocate(GameObject poolObject, int objectIndex, Vector3 position, Quaternion rotation)
        {
            poolObject.transform.position = position;
            poolObject.transform.rotation = rotation;
            poolObject.SetActive(true);
        }
    }
}