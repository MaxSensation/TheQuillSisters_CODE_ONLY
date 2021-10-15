// Primary Author : Viktor Dahlberg - vida6631

using System;
using Environment.Trigger;
using Framework;
using Framework.SaveSystem;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Pickup
{
	/// <summary>
	///     The checkpoint.
	/// </summary>
	public class EyeOfHorus : MonoBehaviour
    {
        [Header("Save Link")]
        
        [SerializeField] 
        private GameEvent gameSaveRequested = default;
        
        [Header("Global Variables")] 
        
        [SerializeField]
        private ScriptObjVar<EDifficulty> currentDifficulty = default;

        [Header("Global Player Variables")] 
        
        [SerializeField]
        private ScriptObjVar<Vector3> playerRespawnPosition = default;
        [SerializeField] 
        private ScriptObjVar<Quaternion> playerRespawnRotation = default;
        [SerializeField] 
        private ScriptObjVar<float> playerCurrentHealth = default;
        [SerializeField] 
        private ScriptObjVar<float> playerMaxHealth = default;

        [Header("Per-eye Variables")] 
        
        [SerializeField]
        private ScriptObjRef<Vector3> targetRespawnPosition = default;
        [SerializeField] 
        private ScriptObjRef<Quaternion> targetRespawnRotation = default;
        [SerializeField]
        private bool useCustomHealthRecovery = default;
        [SerializeField] [Range(0f, 1f)] [Tooltip("At 1f, will add player max health to current player health")]
        private float customHealthRecoveryAmount = default;

        [Header("State Stuff - WIP version for playtest")]
        
        [Tooltip("If no Area Trigger is assigned, the child Area Trigger will be used.")]
        [SerializeField]
        private AreaTrigger areaTrigger = default;
        [SerializeField] 
        private ScriptObjVar<bool> triggered = default;
        [SerializeField] 
        private MeshRenderer meshRenderer = default;
        [SerializeField] [ColorUsage(true, true)] 
        private Color untriggeredColor = default;
        [SerializeField] [ColorUsage(true, true)]
        private Color triggeredColor = default;

        public static Action HealedPlayer;
        public Action OnActivated;
        private Animator _anim;
        private Material[] _materials;

        private void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
            if (areaTrigger is null)
            {
                areaTrigger = GetComponentInChildren<AreaTrigger>();
                areaTrigger.gameObject.SetActive(true);
            }

            _materials = meshRenderer.materials;
            foreach (var material in _materials)
            {
                material.SetFloat("_EmissiveExposureWeight", 0f);
            }

            SetState(triggered);
        }

        private void OnEnable()
        {
            areaTrigger.OnTrigger += _ => Trigger();
        }

        private void OnDisable()
        {
            areaTrigger.OnTrigger -= _ => Trigger();
        }

        private void OnApplicationQuit()
        {
            SetState(false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            var pos = transform.TransformPoint(targetRespawnPosition.Value);
            Gizmos.DrawWireSphere(pos, 0.5f);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(pos, 0.1f);
        }

        /// <summary>
        ///     Triggers the EOH and all involved functionality.
        /// </summary>
        private void Trigger()
        {
            if (!triggered)
            {
                OnActivated?.Invoke();
                SetState(true);
                playerRespawnPosition.value = targetRespawnPosition.useConstant
                    ? transform.TransformPoint(targetRespawnPosition.constantValue)
                    : targetRespawnPosition.Value;
                playerRespawnRotation.value = targetRespawnRotation.Value;
                playerCurrentHealth.value = Mathf.Clamp(playerCurrentHealth + GetHealAmount(), 0f, playerMaxHealth);
                HealedPlayer.Invoke();
                gameSaveRequested.Raise();
            }
        }

        /// <summary>
        ///     Adds a Debug save function
        /// </summary>
        [ContextMenu("Debug SaveGame")]
        private void DebugSaveGame()
        {
            playerRespawnPosition.value = targetRespawnPosition.useConstant
                ? transform.TransformPoint(targetRespawnPosition.constantValue)
                : targetRespawnPosition.Value;
            playerRespawnRotation.value = targetRespawnRotation.Value;
            gameSaveRequested.Raise();
            FindObjectOfType<SaveManager>().DebugSaveGame();
        }

        /// <summary>
        ///     Calculates how much healing should be done based on difficulty or custom setting.
        /// </summary>
        /// <returns>Resultant healing amount.</returns>
        private float GetHealAmount()
        {
            if (useCustomHealthRecovery)
            {
                return playerMaxHealth * customHealthRecoveryAmount;
            }

            switch (currentDifficulty.value)
            {
                case EDifficulty.EASY: return playerMaxHealth / 2f;
                case EDifficulty.NORMAL: return playerMaxHealth / 4f;
                case EDifficulty.HARD: return playerMaxHealth / 10f;
                case EDifficulty.CURSED: return 0f;
                default: return 0f;
            }
        }

        /// <summary>
        ///     Sets state of EOH between triggered and un-triggered.
        /// </summary>
        /// <param name="triggered">Whether the state should be "triggered".</param>
        private void SetState(bool triggered)
        {
            this.triggered.value = triggered;
            foreach (var material in _materials)
                if (triggered)
                {
                    material.SetColor("_EmissiveColor", triggeredColor);
                    if (_anim != null)
                    {
                        _anim.enabled = false;
                    }
                }
                else
                {
                    material.SetColor("_EmissiveColor", untriggeredColor);
                    if (_anim != null)
                    {
                        _anim.enabled = true;
                    }
                }
        }
    }
}