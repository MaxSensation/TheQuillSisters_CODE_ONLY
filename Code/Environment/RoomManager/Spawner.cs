// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using System.Linq;
using Entity.HealthSystem;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.RoomManager
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private float spread = default;
        [SerializeField]
        private LayerMask spawnLayerMask = default;
        [SerializeField]
        private Enemy[] enemyEnum = default;
        [SerializeField]
        private GameObject[] enemyGameObject = default;

        private readonly Dictionary<Enemy, GameObject> _enemyEnumGameObjectPairs = new Dictionary<Enemy, GameObject>();

        public List<GameObject> AliveEnemies { get; private set; }

        private List<SpawnPoint> EnemySpawnPoints { get; set; }

        private void Awake()
        {
            EnemySpawnPoints = new List<SpawnPoint>();
            AliveEnemies = new List<GameObject>();
            foreach (var enemySpawnPoint in GetComponentsInChildren<SpawnPoint>())
            {
                EnemySpawnPoints.Add(enemySpawnPoint);
            }

            if (EnemySpawnPoints.Count == 0)
            {
                throw new Exception("No spawnPoints in round!");
            }

            for (var i = 0; i < enemyEnum.Length; i++)
            {
                _enemyEnumGameObjectPairs.Add(enemyEnum[i], enemyGameObject[i]);
            }
        }

        public void KillAll()
        {
            foreach (var t in AliveEnemies.Where(t => t.activeSelf))
            {
                t.GetComponent<EnemyHealth>().TakeDamage(float.MaxValue);
            }
        }

        public void SpawnEnemies(Enemy enemyType, int amount)
        {
            var currentSpawned = 0;
            var spawnList = EnemySpawnPoints;
            while (currentSpawned != amount)
            {
                spawnList.Sort((s1, s2) => s1.amountSpawned.CompareTo(s2.amountSpawned));
                foreach (var spawn in EnemySpawnPoints.TakeWhile(spawn => currentSpawned != amount))
                {
                    if (spawn.SpawnType == SpawnType.SingleUnit)
                    {
                        spawn.Used = true;
                    }

                    if (enemyType.ToString() != "0")
                    {
                        var enemyGO = _enemyEnumGameObjectPairs[enemyType];
                        var enemyCollider = enemyGO.GetComponent<CapsuleCollider>();
                        var entityWorldCenter = Room.LocateSpawnLocation(spawn.transform, spawn.SpawnType, enemyCollider);
                        if ((enemyType & Enemy.Scarabs) != 0 || CanSpawn(entityWorldCenter, enemyCollider))
                        {
                            AliveEnemies.Add(Instantiate(enemyGO, entityWorldCenter, Quaternion.identity));
                            currentSpawned++;
                            spawn.amountSpawned++;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Checks if the Entity can spawn in the desired area.
        /// </summary>
        /// <param name="entityWorldCenter">The center of the wanted spawned Entity.</param>
        /// <param name="enemyCollider">The CapsuleCollider of the wanted to spawned Entity</param>
        /// <returns>Returns true if the entity can Spawn at that location</returns>
        private bool CanSpawn(Vector3 entityWorldCenter, CapsuleCollider enemyCollider)
        {
            if (!HasFloor(new Vector3(entityWorldCenter.x, entityWorldCenter.y - enemyCollider.height / 2, entityWorldCenter.z)))
            {
                return false;
            }

            var center = enemyCollider.center;
            var bottom = new Vector3(center.x, center.y - enemyCollider.height / 2 + (enemyCollider.radius + spread), enemyCollider.center.z);
            var top = new Vector3(center.x, center.y + enemyCollider.height / 2 - (enemyCollider.radius - spread), enemyCollider.center.z);
            var hits = Physics.OverlapCapsule(top + entityWorldCenter, bottom + entityWorldCenter, enemyCollider.radius + spread, spawnLayerMask);
            return hits.Length == 0;
        }

        /// <summary>
        ///     Checks if spawningPosition has any floor.
        /// </summary>
        /// <param name="spawnFloorLocation"></param>
        /// <returns>Returns true if the spawner has any floor.</returns>
        private bool HasFloor(Vector3 spawnFloorLocation)
        {
            Physics.Raycast(spawnFloorLocation, Vector3.down, out var hit, 0.2f, spawnLayerMask);
            return hit.collider;
        }

        public Vector3 GetRandomSpawnPoint(CapsuleCollider enemyCollider)
        {
            var position = EnemySpawnPoints[Random.Range(0, EnemySpawnPoints.Count)].transform.position;
            return CanSpawn(position + enemyCollider.height / 2 * Vector3.up, enemyCollider) ? position : Vector3.zero;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Adds a SpawnPoint as a child to this Round, is called from the editor script
        /// </summary>
        public void AddSpawnPoint()
        {
            var instance = new GameObject {name = "SpawnPoint " + (gameObject.GetComponentsInChildren<SpawnPoint>().Length + 1)};
            instance.transform.localScale = new Vector3(10f, 2, 10f);
            var spawnInstance = instance.AddComponent<SpawnPoint>();
            Selection.activeGameObject = spawnInstance.gameObject;
            instance.transform.parent = transform;
        }
#endif
    }
}