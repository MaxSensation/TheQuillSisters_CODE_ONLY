// Primary Author : Viktor Dahlberg - vida6631

using System.Linq;
using Entity.AI;
using UnityEngine;
using VFX;

namespace Environment
{
    public class SpawnPool : ObjectPool
    {
        private SpawnController[] _spawnControllers;
        private GameObject _target;
        private int _allocatedIndex;
        private Vector3 _decalInitialSize;

        protected override void Init()
        {
            _spawnControllers = (from obj in managed select obj.GetComponent<SpawnController>()).ToArray();
            Enemy.OnSpawned += SetTarget;
        }

        private void SetTarget(GameObject gameObject)
        {
            _target = gameObject;
            RequestAt(gameObject.transform.position + Vector3.up, Quaternion.identity);
        }

        protected override void Allocate(GameObject poolObject, int objectIndex, Vector3 position, Quaternion rotation)
        {
            var spawnable = _target.GetComponent<ISpawnable>();
            if (spawnable == null)
            {
                return;
            }

            var meshRenderer = _target.GetComponentInChildren<Renderer>();
            meshRenderer.enabled = false;
            _spawnControllers[objectIndex].ToEnable = meshRenderer;
            poolObject.transform.localScale = spawnable.GetSpawnScale();
            _spawnControllers[objectIndex].SetOtherScale();
            base.Allocate(poolObject, objectIndex, position, rotation);
        }
    }
}