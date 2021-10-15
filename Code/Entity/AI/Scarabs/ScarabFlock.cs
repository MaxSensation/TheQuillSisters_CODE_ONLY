//Author: Andreas Berzelius - anbe5918

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.AI.Scarabs
{
    public class ScarabFlock : MonoBehaviour
    {
        [SerializeField, Tooltip("How many followers should spawn")]
        private int scarabsToSpawn = default;
        [SerializeField, Tooltip("First prefab to spawn")]
        private GameObject scarabLeaderPrefab = default;
        [SerializeField, Tooltip("Follows the leader!")]
        private GameObject scarabPrefab = default;
        [SerializeField]
        private float minSpawnRadius = 1f;
        [SerializeField]
        private float maxSpawnRadius = 2f;

        private int _currentAlive;
        private GameObject[] _scarabFlock;
        private GameObject _localLeaderScarab = default;

        private void Start()
        {
            _scarabFlock = new GameObject[scarabsToSpawn];
            _localLeaderScarab = Instantiate(scarabLeaderPrefab, transform.position, Quaternion.identity, this.gameObject.transform);
            _localLeaderScarab.GetComponent<Scarab>().flock = this;
            for (int i = 0; i < scarabsToSpawn; i++)
            {
                scarabPrefab.GetComponent<Scarab>().leaderObj = _localLeaderScarab.GetComponent<Scarab>();
                var scarab = Instantiate(scarabPrefab, GetRandomPosition(), Quaternion.identity, this.gameObject.transform);
                scarab.GetComponent<Scarab>().flock = this;
                _scarabFlock[i] = scarab;
            }
            _currentAlive = scarabsToSpawn + 1;
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomXZ = Random.insideUnitSphere.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 randomDir = new Vector3(randomXZ.x, 0f, randomXZ.z);
            return randomDir += transform.position;
        }

        public void ScarabDied()
        {
            _currentAlive--;
            if (_currentAlive == 0)
            {
                Enemy.OnDied?.Invoke(gameObject);
            }
        }
    }
}
