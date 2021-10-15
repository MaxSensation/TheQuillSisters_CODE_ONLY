// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.IO;
using Combat.Ability;
using Combat.ConditionSystem.Condition;
using Combat.Melee;
using Entity.AI.Attacks;
using Entity.AI.Bosses.Khonsu.Attacks;
using Entity.AI.Bosses.TwinofRa.Attacks;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framework
{
    public class ParametersManager : MonoBehaviour
    {
        private static string _defaultParameterPath;
        private static string _parameterPath;
        private static string _pendingDefaultParameterPath;
        private static string _oldParameterPath;
        private static ParameterData _defaultParameterData;
        private static ParameterData _parameterData;
        private static ParameterData _pendingDefaultData;

        [SerializeField]
        private ScriptObjVar<float> playerMaxHealth = default;
        [SerializeField]
        private ScriptObjVar<float> playerMaxSoulEssence = default;
        [SerializeField]
        private ScriptObjHeavyAttack playerHeavyAttack = default;
        [SerializeField]
        private ScriptObjGroundLightAttack playerFirstLightAttack = default;
        [SerializeField]
        private ScriptObjGroundLightAttack playerSecondLightAttack = default;
        [SerializeField]
        private ScriptObjGroundLightAttack playerThirdLightAttack = default;
        [SerializeField]
        private ScriptObjAbilityBase playerAbilityBeam = default;
        [SerializeField]
        private ScriptObjAbilityBase playerAbilityNova = default;
        [SerializeField]
        private InvincibilityCondition playerInvincibilityCondition = default;
        [SerializeField]
        private ScriptObjVar<float> mummyMeleeMaxHealth = default;
        [SerializeField]
        private MeleeAttack mummyMeleeAttack = default;
        [SerializeField]
        private ScriptObjVar<float> mummyRangeMaxHealth = default;
        [SerializeField]
        private RangedAttack mummyRangeAttack = default;
        [SerializeField]
        private RangedAttack mummyRangeKhonsuAttack = default;
        [SerializeField]
        private ScriptObjVar<float> mummyGiantMaxHealth = default;
        [SerializeField]
        private MeleeAttack mummyGiantAttack = default;
        [SerializeField]
        private ScriptObjVar<float> scarabMaxHealth = default;
        [SerializeField]
        private MeleeAttack scarabAttack = default;
        [SerializeField]
        private ScriptObjVar<float> scarabLeaderMaxHealth = default;
        [SerializeField]
        private MeleeAttack scarabLeaderAttack = default;
        [SerializeField]
        private ScriptObjVar<float> khonsuMaxHealth = default;
        [SerializeField]
        private TelegraphedNova khonsuAttack = default;
        [SerializeField]
        private ScriptObjVar<float> twinOfRaPhaseOneAttackHealth = default;
        [SerializeField]
        private ScriptObjVar<float> twinOfRaPhaseTwoAttackHealth = default;
        [SerializeField]
        private TwinOfRaBeam twinOfRaPhaseOneAttack = default;
        [SerializeField]
        private TwinOfRaBeam twinOfRaPhaseTwoAttack = default;
        [SerializeField]
        private ScriptObjVar<float> zoeMaxHealth = default;

        private void Awake()
        {
            GetFiles();

            if (_defaultParameterData == null)
            {
                return;
            }

            if (_pendingDefaultData != null)
            {
                if (HasValidVersion(_pendingDefaultData))
                {
                    SetPendingDefaultParameters();
                    CopyDefaultToAppdata();
                }
                else
                {
                    FixVersionMismatched(_pendingDefaultParameterPath);
                }
            }
            else if (_parameterData != null)
            {
                if (!HasValidVersion(_parameterData))
                {
                    FixVersionMismatched(_parameterPath);
                }
            }
            else
            {
                CopyDefaultToAppdata();
            }

            SetValues(GetJson(_parameterPath));
        }

        private static void FixVersionMismatched(string path)
        {
            Debug.LogWarning($"Version mismatched between {path} and {_defaultParameterPath}! A copy of your old parameters has been saved here: {Application.persistentDataPath} and the original parameters has been overriden!");
            if (File.Exists(_oldParameterPath))
            {
                File.Delete(_oldParameterPath);
            }

            File.Copy(path, _oldParameterPath);
            File.Delete(path);
            CopyDefaultToAppdata();
        }

        private static bool HasValidVersion(ParameterData data)
        {
            return _defaultParameterData.versionNumber <= data.versionNumber;
        }

        private static void GetFiles()
        {
            _defaultParameterPath = Path.Combine(Application.streamingAssetsPath, "DefaultParameterSettings.json");
            _parameterPath = Path.Combine(Application.persistentDataPath, "ParameterSettings.json");
            _pendingDefaultParameterPath =
                Path.Combine(Application.persistentDataPath, "DefaultParameterSettings.json");
            _oldParameterPath = Path.Combine(Application.persistentDataPath, "OldParameterSettings.json");
            if (File.Exists(_defaultParameterPath))
            {
                _defaultParameterData = GetJson(_defaultParameterPath);
            }
            else
            {
                Debug.LogWarning(
                    $"Failed to find the file: DefaultParameterSettings.json at path: {_defaultParameterPath}");
            }

            if (File.Exists(_parameterPath))
            {
                _parameterData = GetJson(_parameterPath);
            }

            if (File.Exists(_pendingDefaultParameterPath))
            {
                _pendingDefaultData = GetJson(_pendingDefaultParameterPath);
            }
        }

        private static ParameterData GetJson(string path)
        {
            return JsonUtility.FromJson<ParameterData>(File.ReadAllText(path));
        }

        private static void CopyDefaultToAppdata()
        {
            File.Copy(_defaultParameterPath, _parameterPath);
        }

        private static void SetPendingDefaultParameters()
        {
            _pendingDefaultData.versionNumber = Math.Round(_pendingDefaultData.versionNumber + 0.01f, 2);
            File.Delete(_defaultParameterPath);
            File.WriteAllText(_defaultParameterPath, JsonUtility.ToJson(_pendingDefaultData, true));
            File.Delete(_pendingDefaultParameterPath);
        }

        private void SetValues(ParameterData parameterData)
        {
            // PlayerData
            // -- Stats
            playerMaxHealth.value = parameterData.playerMaxHealth;
            playerMaxSoulEssence.value = parameterData.playerMaxSoulEssence;
            playerInvincibilityCondition.SetIframeTime(parameterData.playerDashIframeTimeMilliSeconds);
            // -- weapons
            playerHeavyAttack.SetDamage(parameterData.playerHeavyAttackDamage);
            playerHeavyAttack.SetDamageMultiplier(parameterData.playerHeavyAttackDamageMultiplier);
            playerHeavyAttack.SetDamageFullyChargedMultiplier(parameterData
                .playerHeavyAttackDamageFullyChargedMultiplier);
            playerFirstLightAttack.SetDamage(parameterData.playerFirstLightAttackDamage);
            playerSecondLightAttack.SetDamage(parameterData.playerSecondLightAttackDamage);
            playerThirdLightAttack.SetDamage(parameterData.playerThirdLightAttackDamage);
            // -- abilities
            playerAbilityBeam.SetDamage(parameterData.playerAbilityBeamDamage);
            playerAbilityBeam.SetCost(parameterData.playerAbilityBeamCost);
            playerAbilityNova.SetDamage(parameterData.playerAbilityNovaDamage);
            playerAbilityNova.SetCost(parameterData.playerAbilityNovaCost);
            // MummyMelee
            mummyMeleeMaxHealth.value = parameterData.mummyMeleeMaxHealth;
            mummyMeleeAttack.SetDamage(parameterData.mummyMeleeAttackDamage);
            // MummyRange
            mummyRangeMaxHealth.value = parameterData.mummyRangeMaxHealth;
            mummyRangeAttack.SetDamage(parameterData.mummyRangeAttackDamage);
            // MummyRangeKhonsu
            mummyRangeKhonsuAttack.SetDamage(parameterData.mummyRangeKhonsuAttackDamage);
            // MummyGiant
            mummyGiantMaxHealth.value = parameterData.mummyGiantMaxHealth;
            mummyGiantAttack.SetDamage(parameterData.mummyGiantAttackDamage);
            // Scarabs
            scarabMaxHealth.value = parameterData.scarabMaxHealth;
            scarabAttack.SetDamage(parameterData.scarabAttackDamge);
            // ScarabLeader
            scarabLeaderMaxHealth.value = parameterData.scarabLeaderMaxHealth;
            scarabLeaderAttack.SetDamage(parameterData.scarabLeaderAttackDamge);
            // Boss 1 - Khonsu
            khonsuMaxHealth.value = parameterData.khonsuMaxHealth;
            khonsuAttack.SetDamage(parameterData.khonsuAttackDamge);
            // Boss 2 - TwinsofRa
            twinOfRaPhaseOneAttackHealth.value = parameterData.twinOfRaPhaseOneAttackHealth;
            twinOfRaPhaseTwoAttackHealth.value = parameterData.twinOfRaPhaseTwoAttackHealth;
            twinOfRaPhaseOneAttack.SetDamage(parameterData.twinOfRaPhaseOneAttackDamge);
            twinOfRaPhaseTwoAttack.SetDamage(parameterData.twinOfRaPhaseTwoAttackDamage);
            // Boss 3 - Zoe
            zoeMaxHealth.value = parameterData.zoeMaxHealth;
        }

        private class ParameterData
        {
            public int khonsuAttackDamge;
            public float khonsuMaxHealth;
            public int mummyGiantAttackDamage;
            public float mummyGiantMaxHealth;
            public int mummyMeleeAttackDamage;
            public float mummyMeleeMaxHealth;
            public int mummyRangeAttackDamage;
            public int mummyRangeKhonsuAttackDamage;
            public float mummyRangeKhonsuMaxHealth;
            public float mummyRangeMaxHealth;
            public float playerAbilityBeamCost;
            public int playerAbilityBeamDamage;
            public float playerAbilityNovaCost;
            public int playerAbilityNovaDamage;
            public float playerDashIframeTimeMilliSeconds;
            public int playerFirstLightAttackDamage;
            public int playerHeavyAttackDamage;
            public double playerHeavyAttackDamageFullyChargedMultiplier;
            public double playerHeavyAttackDamageMultiplier;
            public float playerMaxHealth;
            public float playerMaxSoulEssence;
            public int playerSecondLightAttackDamage;
            public int playerThirdLightAttackDamage;
            public int scarabAttackDamge;
            public int scarabLeaderAttackDamge;
            public float scarabLeaderMaxHealth;
            public float scarabMaxHealth;
            public int twinOfRaPhaseOneAttackDamge;
            public float twinOfRaPhaseOneAttackHealth;
            public int twinOfRaPhaseTwoAttackDamage;
            public float twinOfRaPhaseTwoAttackHealth;
            public double versionNumber;
            public float zoeMaxHealth;
        }
    }
}