// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.Player;
using Environment.Trigger;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment.RoomManager
{
    /// <summary>
    ///     A Scene can have many Rooms and a Room can have many Rounds and each Round can have many EnemySpawnPoints.
    ///     This is the main class for the whole RoomManager and the RoomManager is the Spawner for our Rooms.
    ///     This class can spawn enemies of different types specified by the user at specific places placed by the user.
    ///     SpawnLayerMask is the layer that the Entity can spawn on and the layer that the Entity can not spawn into.
    ///     Spread is the distance between each Entity spawned by this system.
    /// </summary>
    public class Room : MonoBehaviour
    {
        #region Editor

        /// <summary>
        ///     Adds a Round to this GameObject
        /// </summary>
        public void AddRound()
        {
            var id = gameObject.GetComponentsInChildren<Round>().Length + 1;
            var instance = new GameObject {name = "Round " + id};
            instance.AddComponent<Round>();
            instance.transform.parent = transform;
        }

        #endregion

        #region SerializeFields

        [SerializeField]
        private AreaTrigger areaTrigger = default;
        [SerializeField]
        private GameEvent gameEvent = default;
        [SerializeField]
        private RoomKeyTrigger[] roomKeys = default;
        [SerializeField]
        private bool spawnTest = default;
        [SerializeField]
        private bool keyOnly = default;
        [SerializeField]
        private LayerMask spawnLayerMask = default;
        [SerializeField] [Range(0, 1)] 
        private float spread = default;
        
        [Header("Dictionary Builder")] 
        
        [SerializeField]
        private Enemy[] enemyEnum = default;
        [SerializeField] 
        private GameObject[] enemyGameObject = default;

        #endregion

        #region Events

        public static Action<int> OnInitKey;
        public static Action OnPickedKey;
        public Action OnRoomStarted;
        public Action OnRoomCompleted;
        public Action OnRoundCompleted;

        #endregion

        #region ListAndDictionaries

        private Dictionary<Enemy, GameObject> _enemyEnumGameObjectPairs;
        private Dictionary<Enemy, int> _enemiesLeftToSpawn;
        private List<Round> _rounds;
        private List<Enemy> _chosenEnemies;
        private List<GameObject> _aliveEnemies;

        #endregion

        #region PrivateVariables

        private const uint Enabled = 1;
        private bool _roomActive;
        private bool _isRoundActive;
        private Round _currentRound;
        private int _currentRoundNr;
        private int _pickedUpKeys;

        #endregion

        #region Init

        private void Awake()
        {
            _rounds = new List<Round>();
            _chosenEnemies = new List<Enemy>();
            _aliveEnemies = new List<GameObject>();
            _enemyEnumGameObjectPairs = new Dictionary<Enemy, GameObject>();
            _enemiesLeftToSpawn = new Dictionary<Enemy, int>();
            CreateDictionaries();
        }

        private void Start()
        {
            if (keyOnly && roomKeys.Length == 0)
            {
                throw new Exception("KeyOnly selected but no keys is in the list?");
            }

            PlayerController.OnPlayerDied += DestroyAllEnemies;
            CheckSpawnPoints();
            if (spawnTest)
            {
                StartNextRound();
            }

            if (areaTrigger != null)
            {
                areaTrigger.OnTrigger += _ => StartRoom();
            }

            if (gameEvent != null)
            {
                gameEvent.OnEvent += StartRoom;
            }

            Entity.AI.Enemy.OnDied += OnEnemyKilled;
        }

        private void StartRoom()
        {
            OnRoomStarted?.Invoke();
            StartNextRound();
        }

        public bool HasKeys()
        {
            return roomKeys.Length > 0;
        }

        private void DestroyAllEnemies()
        {
            _aliveEnemies.ForEach(Destroy);
        }

        /// <summary>
        ///     Unsubscribe to future EnemyClass
        /// </summary>
        private void OnDestroy()
        {
            if (gameEvent != null)
            {
                gameEvent.OnEvent -= StartRoom;
            }

            Entity.AI.Enemy.OnDied -= OnEnemyKilled;
        }


        /// <summary>
        ///     Kills all spawned enemies to test the SpawnSystem
        /// </summary>
        private async void DebugModeKillAll()
        {
            await Task.Delay(TimeSpan.FromSeconds(5f));
            Debug.Log($"RoomManager DebugMode: Killing {_aliveEnemies.Count} spawned enemies.");
            _aliveEnemies.ForEach(Destroy);
            _aliveEnemies.Clear();
            if (_enemiesLeftToSpawn.Values.Sum() > 0)
            {
                Debug.Log($"RoomManager DebugMode: Has {_enemiesLeftToSpawn.Values.Sum()} enemies to spawn.");
                DebugModeKillAll();
            }

            OnEnemyKilled(gameObject);
        }

        /// <summary>
        ///     Checks if populated enemies has a dedicated SpawnPoint or else throw a readable Execution.
        /// </summary>
        private void CheckSpawnPoints()
        {
            foreach (var round in _rounds)
            {
                foreach (var enemySpawnPoint in round.EnemySpawnPoints)
                {
                    if (enemySpawnPoint.Enemy.ToString().Equals("-1"))
                    {
                        continue;
                    }

                    var enumLength = enemyEnum.Length;
                    for (var i = 0; i < enumLength; i++)
                    {
                        var enemyType = (uint) enemySpawnPoint.Enemy >> (enumLength - 1 - i);
                        var result = enemyType & 1;
                        if (result == Enabled) _chosenEnemies.Add((Enemy) (result << (enumLength - 1 - i)));
                    }
                }

                CheckSpawnPoint(round.name, Enemy.Jackal, round.Jackals);
                CheckSpawnPoint(round.name, Enemy.Scarabs, round.Scarabs);
                CheckSpawnPoint(round.name, Enemy.MummyGiant, round.MummyGiant);
                CheckSpawnPoint(round.name, Enemy.MummyMelee, round.MummyMelee);
                CheckSpawnPoint(round.name, Enemy.MummyRanged, round.MummyRanged);
                _chosenEnemies.Clear();
            }
        }

        /// <summary>
        ///     This is just a help Method for the CheckSpawnPoints and will throw the Execution if the EnemyType is missing
        ///     SpawnPoints.
        /// </summary>
        /// <param name="round">The current checked Round</param>
        /// <param name="enemyType">The EnemyType That will be checked</param>
        /// <param name="wantToSpawn">The amount of Enemies of that Enemy Type specified by the user</param>
        /// <exception cref="Exception">A error for the user to see that the room is missing a SpawnPoint</exception>
        private void CheckSpawnPoint(string round, Enemy enemyType, int wantToSpawn)
        {
            if (wantToSpawn > 0 && !_chosenEnemies.Contains(enemyType))
            {
                throw new Exception(
                    $"Missing spawnpoints for {enemyType} in {round} inside the {gameObject.name} " +
                    $"or have selected the wrong enemy type amount in {round}!");
            }
        }

        /// <summary>
        ///     Populates all the necessary Dictionaries and adding getting the rounds in all children.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void CreateDictionaries()
        {
            if (enemyEnum.Length != enemyGameObject.Length)
            {
                throw new Exception("Enum length and GameObject length are different. Unable to build dictionary.");
            }

            for (var i = 0; i < enemyEnum.Length; i++)
            {
                _enemyEnumGameObjectPairs.Add(enemyEnum[i], enemyGameObject[i]);
            }

            foreach (var enemy in enemyEnum)
            {
                _enemiesLeftToSpawn.Add(enemy, 0);
            }

            foreach (var round in gameObject.GetComponentsInChildren<Round>())
            {
                _rounds.Add(round);
            }

            _rounds = _rounds.OrderBy(round => round.name).ToList();
            foreach (var key in roomKeys)
            {
                key.OnPickedUp += OnKeyPickedUp;
            }
        }

        private void OnKeyPickedUp()
        {
            if (_pickedUpKeys == 0)
            {
                OnInitKey?.Invoke(roomKeys.Length);
            }

            OnPickedKey?.Invoke();
            _pickedUpKeys++;
            if (_pickedUpKeys == roomKeys.Length && (_currentRoundNr > _rounds.Count - 1 && !_roomActive || keyOnly))
            {
                OnRoomCompleted?.Invoke();
            }
        }

        #endregion

        #region RoomLogic

        /// <summary>
        ///     To start next Round in the list
        /// </summary>
        /// <exception cref="Exception">Throws a Execution if no rounds is created and the game is started.</exception>
        private void StartNextRound()
        {
            if (_rounds.Count == 0)
            {
                throw new Exception($"{gameObject.name} has no Rounds!");
            }

            if (_currentRoundNr > _rounds.Count - 1)
            {
                if (spawnTest)
                {
                    Debug.Log($"RoomManager DebugMode: {gameObject.name} Completed!");
                }

                if (_pickedUpKeys == roomKeys.Length)
                {
                    OnRoomCompleted?.Invoke();
                }

                _roomActive = false;
            }
            else
            {
                _roomActive = true;
                _currentRound = _rounds[_currentRoundNr];
                if (spawnTest)
                {
                    Debug.Log($"RoomManager DebugMode: {_currentRound.name} started");
                    DebugModeKillAll();
                }

                _enemiesLeftToSpawn[Enemy.MummyMelee] = _currentRound.MummyMelee;
                _enemiesLeftToSpawn[Enemy.MummyRanged] = _currentRound.MummyRanged;
                _enemiesLeftToSpawn[Enemy.MummyGiant] = _currentRound.MummyGiant;
                _enemiesLeftToSpawn[Enemy.Jackal] = _currentRound.Jackals;
                _enemiesLeftToSpawn[Enemy.Scarabs] = _currentRound.Scarabs;
                _currentRoundNr++;
                _isRoundActive = true;
            }
        }

        /// <summary>
        ///     This will be called when any enemy dies.
        /// </summary>
        /// <param name="entity">The Entity that died.</param>
        private void OnEnemyKilled(GameObject entity)
        {
            if (_roomActive)
            {
                _aliveEnemies.Remove(entity);
                if (_aliveEnemies.Count == 0 && _enemiesLeftToSpawn.Values.Sum() == 0)
                {
                    _isRoundActive = false;
                    OnRoundCompleted?.Invoke();
                    _rounds.ForEach(r => r.EnemySpawnPoints.ForEach(s => s.Used = false));
                    if (spawnTest)
                    {
                        Debug.Log($"RoomManager DebugMode: {_currentRound.name} Completed!");
                    }

                    StartNextRound();
                }
            }
        }

        /// <summary>
        ///     Spawn Enemies if any enemies is left to spawn.
        /// </summary>
        private void Update()
        {
            if (_isRoundActive && _enemiesLeftToSpawn.Values.Sum() > 0)
            {
                SpawnEnemies();
            }
        }

        #endregion

        #region Spawn

        /// <summary>
        ///     Spawns the enemies in the spawnpoints created under the Rounds.
        /// </summary>
        private void SpawnEnemies()
        {
            foreach (var spawn in _currentRound.EnemySpawnPoints.Where(spawn => !spawn.Used))
            {
                var enemyType = GetEnemyType(spawn.Enemy);
                if (enemyType.ToString() != "0")
                {
                    var enemyGO = _enemyEnumGameObjectPairs[enemyType];
                    var enemyCollider = enemyGO.GetComponent<CapsuleCollider>();
                    var entityWorldCenter = LocateSpawnLocation(spawn.transform, spawn.SpawnType, enemyCollider);
                    if ((enemyType & Enemy.Scarabs) != 0 || CanSpawn(entityWorldCenter, enemyCollider))
                    {
                        if (spawn.SpawnType == SpawnType.SingleUnit)
                        {
                            spawn.Used = true;
                        }

                        _aliveEnemies.Add(Instantiate(enemyGO, entityWorldCenter, Quaternion.identity,
                            gameObject.transform));
                        _enemiesLeftToSpawn[enemyType]--;
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
            if (!HasFloor(new Vector3(entityWorldCenter.x, entityWorldCenter.y - enemyCollider.height / 2,
                entityWorldCenter.z)))
            {
                return false;
            }

            var center = enemyCollider.center;
            var bottom = new Vector3(center.x, center.y - enemyCollider.height / 2 + (enemyCollider.radius - spread),
                enemyCollider.center.z);
            var top = new Vector3(center.x, center.y + enemyCollider.height / 2 - (enemyCollider.radius - spread),
                enemyCollider.center.z);
            var hits = Physics.OverlapCapsule(top + entityWorldCenter + Vector3.down * spread,
                bottom + entityWorldCenter + Vector3.up * spread, enemyCollider.radius + spread, spawnLayerMask);
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

        /// <summary>
        ///     Gets a new SpawnLocation from the SpawnPoint depending on the SpawnType.
        /// </summary>
        /// <param name="spawnTransform">The transform of the SpawnPoint.</param>
        /// <param name="spawnType">The Type of Spawn to use.</param>
        /// <param name="enemyCollider">The CapsuleCollider attached to the Entity.</param>
        /// <returns>A SpawnLocation</returns>
        internal static Vector3 LocateSpawnLocation(Transform spawnTransform, SpawnType spawnType,
            CapsuleCollider enemyCollider)
        {
            var spawnPosition = spawnTransform.position;
            if (spawnType == SpawnType.SingleUnit)
            {
                return new Vector3(spawnPosition.x, spawnPosition.y + enemyCollider.height / 2, spawnPosition.z);
            }

            var localScale = spawnTransform.localScale;
            var radius = enemyCollider.radius;
            return new Vector3(
                Random.Range(spawnPosition.x - localScale.x / 2 + radius,
                    spawnPosition.x + localScale.x / 2 - radius),
                spawnPosition.y + enemyCollider.height / 2,
                Random.Range(spawnPosition.z - localScale.z / 2 + radius,
                    spawnPosition.z + localScale.z / 2 - radius));
        }

        /// <summary>
        ///     Gets a EnemyType based on the wanted enemy at that location.
        /// </summary>
        /// <param name="enemyOptions">What can be spawned on selected SpawnPoint.</param>
        /// <returns>Returns a Enemy Type based on wanted enemy.</returns>
        private Enemy GetEnemyType(Enemy enemyOptions)
        {
            switch (enemyOptions.ToString())
            {
                case "0":
                    return 0;
                case "-1":
                    return enemyEnum[Random.Range(0, enemyEnum.Length)];
            }

            _chosenEnemies.Clear();
            var enumLength = enemyEnum.Length;
            for (var i = 0; i < enumLength; i++)
            {
                var enemyType = (uint) enemyOptions >> (enumLength - 1 - i);
                var result = enemyType & 1;
                if (result == Enabled)
                {
                    var inSelection = (Enemy) (result << (enumLength - 1 - i));
                    if (_enemiesLeftToSpawn[inSelection] > 0)
                    {
                        _chosenEnemies.Add(inSelection);
                    }
                }
            }
            return _chosenEnemies.Count == 0 ? 0 : _chosenEnemies[Random.Range(0, _chosenEnemies.Count)];
        }

        #endregion
    }
}