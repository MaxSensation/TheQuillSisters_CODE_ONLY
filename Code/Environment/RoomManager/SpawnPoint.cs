// Primary Author : Maximiliam Rosén - maka4519

using System;
using UnityEngine;

namespace Environment.RoomManager
{
    /// <summary>
    ///     A Room can have many Rounds and each Round can have many EnemySpawnPoints.
    ///     A SpawnPoint for the RoomManager, can have multi EnemyTypes and has two different spawnTypes Area or single slot
    ///     types.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] [EnumFlag] 
        private Enemy enemy = default;
        [SerializeField] 
        private SpawnType spawnType = default;
        [HideInInspector] 
        public int amountSpawned = default;
        [SerializeField] 
        private bool isInitialized = default;

        public SpawnType SpawnType => spawnType;

        public Enemy Enemy => enemy;
        public bool Used { get; set; }

        /// <summary>
        ///     Draw the gizmo for the SpawnPoint to make it easier for the user to determined what enemy type that will spawn and
        ///     where.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Color of the type of enemy that can spawn on the area
            switch (enemy)
            {
                case Enemy.MummyMelee:
                    Gizmos.color = Color.blue;
                    break;
                case Enemy.MummyRanged:
                    Gizmos.color = Color.yellow;
                    break;
                case Enemy.MummyGiant:
                    Gizmos.color = Color.red;
                    break;
                case Enemy.Jackal:
                    Gizmos.color = Color.green;
                    break;
                case Enemy.Scarabs:
                    Gizmos.color = Color.cyan;
                    break;
                default:
                    Gizmos.color = Color.yellow + Color.blue;
                    break;
            }

            // If nothing is selected choice white
            if (enemy.ToString().Equals("0"))
            {
                Gizmos.color = Color.white;
            }
            // If everything is selected choice magenta
            else if (enemy.ToString().Equals("-1"))
            {
                Gizmos.color = Color.magenta;
            }

            // Get the position where to spawn the gizmo and present the gizmo depending on the type of SpawnType;
            var spawnTransform = transform;
            var spawnScale = spawnTransform.localScale;
            switch (spawnType)
            {
                case SpawnType.SingleUnit:
                    Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(1f, 2f, 1f));
                    break;
                case SpawnType.Area:
                    Gizmos.DrawWireCube(spawnTransform.position + Vector3.up,
                        new Vector3(spawnScale.x, 2, spawnScale.z));
                    break;
            }
        }

        /// <summary>
        ///     Method for updating the position of the SpawnPoint, Used by the editor script.
        /// </summary>
        /// <param name="position">The new position.</param>
        public void SetSpawnPosition(Vector3 position)
        {
            transform.position = position;
        }

        // Get the status of the Initialize
        public bool GetStatus()
        {
            return isInitialized;
        }

        // Set the Initialize status
        public void SetInit()
        {
            isInitialized = true;
        }
    }

    // Enum for Enemy Type
    [Flags]
    public enum Enemy
    {
        MummyMelee = 1,
        MummyRanged = 2,
        MummyGiant = 4,
        Jackal = 8,
        Scarabs = 16
    }

    // Enum for the Spawn Type
    public enum SpawnType
    {
        SingleUnit,
        Area
    }
}