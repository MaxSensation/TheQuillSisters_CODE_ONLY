// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Environment.RoomManager
{
    /// <summary>
    ///     A Room can have many Rounds and each Round can have many EnemySpawnPoints.
    ///     The user can select how many enemies that will be spawned in the SpawnPoints created by this script.
    /// </summary>
    public class Round : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private int mummyMelee = default;
        [SerializeField]
        private int mummyRanged = default;
        [SerializeField]
        private int mummyGiant = default;
        [SerializeField]
        private int jackals = default;
        [SerializeField]
        private int scarabs = default;

        public int MummyMelee => mummyMelee;
        public int MummyRanged => mummyRanged;
        public int MummyGiant => mummyGiant;
        public int Jackals => jackals;
        public int Scarabs => scarabs;

        public List<SpawnPoint> EnemySpawnPoints { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets all the SpawnPoints that are a child to this Round
        /// </summary>
        private void Awake()
        {
            EnemySpawnPoints = new List<SpawnPoint>();
            foreach (var enemySpawnPoint in GetComponentsInChildren<SpawnPoint>())
            {
                EnemySpawnPoints.Add(enemySpawnPoint);
            }

            if (EnemySpawnPoints.Count == 0)
            {
                throw new Exception("No spawnPoints in round!");
            }
        }

#if UNITY_EDITOR
        /// <summary>
        ///     Adds a SpawnPoint as a child to this Round, is called from the editor script
        /// </summary>
        public void AddSpawnPoint()
        {
            var instance = new GameObject
                {name = "SpawnPoint " + (gameObject.GetComponentsInChildren<SpawnPoint>().Length + 1)};
            instance.transform.localScale = new Vector3(10f, 2, 10f);
            var spawnInstance = instance.AddComponent<SpawnPoint>();
            Selection.activeGameObject = spawnInstance.gameObject;
            instance.transform.parent = transform;
        }
#endif

        #endregion
    }
}