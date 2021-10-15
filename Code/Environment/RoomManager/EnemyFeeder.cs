// Primary Author : Viktor Dahlberg - vida6631

using System;
using Entity.HealthSystem;
using UnityEngine;

namespace Environment.RoomManager
{
	/// <summary>
	///     Feeds a spawner with enemies to keep up its specified enemy count.
	/// </summary>
	public class EnemyFeeder : MonoBehaviour
    {
        [SerializeField] 
        private SpawnDesignation[] spawners = default;

        private void OnEnable()
        {
            Health.EntityDied += Refill;
            for (var i = 0; i < spawners.Length; i++)
            {
                spawners[i].spawner.SpawnEnemies(spawners[i].enemy, spawners[i].amount);
            }
        }

        private void OnDisable()
        {
            Health.EntityDied -= Refill;
            for (var i = 0; i < spawners.Length; i++)
            {
                spawners[i].spawner.KillAll();
            }
        }

        private void Refill(GameObject obj)
        {
            for (var i = 0; i < spawners.Length; i++)
                if (spawners[i].spawner.AliveEnemies.Contains(obj))
                {
                    spawners[i].spawner.AliveEnemies.Remove(obj);
                    spawners[i].spawner.SpawnEnemies(spawners[i].enemy, 1);
                    break;
                }
        }

        [Serializable]
        private struct SpawnDesignation
        {
            public Spawner spawner;
            public Enemy enemy;
            public int amount;
        }
    }
}