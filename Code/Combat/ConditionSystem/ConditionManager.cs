// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using System.Linq;
using Framework;
using UnityEngine;

namespace Combat.ConditionSystem
{
	/// <summary>
	///     The manager for all conditions to make it possible to add or remove conditions on any entity during runtime.
	/// </summary>
	public static class ConditionManager
    {
        private static readonly Dictionary<ConditionBase, List<EntityBase>> _conditionDictionary = new Dictionary<ConditionBase, List<EntityBase>>();

        private static ActionGroup<EntityBase> _cacheGroup;
        public static List<ConditionHistoryEntry> ConditionHistoryEntries { get; } = new List<ConditionHistoryEntry>();

        /// <summary>
        ///     Add a condition and entity to the list to make it possible to remove it afterwards.
        /// </summary>
        /// <param name="condition">The condition that the affected entity will get</param>
        /// <param name="applyingEntity">The entity that added this condition to the affected entity</param>
        /// <param name="affectedEntity">The entity that will be affected by the condition</param>
        public static void AddCondition(ConditionBase condition, EntityBase applyingEntity, EntityBase affectedEntity)
        {
            if (IsHighestPriority(condition, affectedEntity))
            {
                CancelAllOfType(condition, affectedEntity);
                // If the condition does not exist add the condition and the entity to the list
                if (!_conditionDictionary.ContainsKey(condition))
                {
                    var entityList = new List<EntityBase> {affectedEntity};
                    _conditionDictionary.Add(condition, entityList);
                    CreateEntry(condition, applyingEntity, affectedEntity, ConditionEntryType.Modified);
                    condition.Modify(applyingEntity, affectedEntity);
                    condition.Modified?.Invoke(applyingEntity, affectedEntity);
                    return;
                }

                // If the condition on the affected entity does not exist then add the condition to that entity
                var entityBases = _conditionDictionary[condition];
                if (!entityBases.Contains(affectedEntity))
                {
                    entityBases.Add(affectedEntity);
                    CreateEntry(condition, applyingEntity, affectedEntity, ConditionEntryType.Modified);
                    condition.Modify(applyingEntity, affectedEntity);
                    condition.Modified?.Invoke(applyingEntity, affectedEntity);
                    _conditionDictionary[condition] = entityBases;
                }
                // If the condition already exist on that entity update that condition on that entity
                else
                {
                    condition.UpdateCondition(applyingEntity, affectedEntity);
                    CreateEntry(condition, null, affectedEntity, ConditionEntryType.Updated);
                    condition.Updated?.Invoke(affectedEntity);
                }
            }
            else
            {
                condition.UpdateCondition(applyingEntity, affectedEntity);
                CreateEntry(condition, null, affectedEntity, ConditionEntryType.Updated);
                condition.Updated?.Invoke(affectedEntity);
            }
        }

        private static void CreateEntry(ConditionBase condition, EntityBase applyingEntity, EntityBase affectedEntity, ConditionEntryType type)
        {
            ConditionHistoryEntries.Add(new ConditionHistoryEntry
            {
                Condition = condition,
                ApplierEntity = applyingEntity,
                AffectedEntity = affectedEntity,
                Timestamp = Time.time,
                Type = type
            });
        }


        /// <summary>
        ///     Remove a entity from the condition list
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="affectedEntity"></param>
        public static void RemoveCondition(ConditionBase condition, EntityBase affectedEntity)
        {
            if (_conditionDictionary.ContainsKey(condition))
            {
                var entityList = _conditionDictionary[condition];
                if (entityList.Contains(affectedEntity))
                {
                    condition.UnModify(affectedEntity);
                    CreateEntry(condition, null, affectedEntity, ConditionEntryType.UnModify);
                    condition.UnModified?.Invoke(affectedEntity);
                    entityList.Remove(affectedEntity);
                    _conditionDictionary[condition] = entityList;
                    if (_conditionDictionary[condition].Count == 0)
                    {
                        _conditionDictionary.Remove(condition);
                    }
                }
            }
        }

        /// <summary>
        ///     Requests the condition to cancel itself.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="affectedEntity"></param>
        public static bool CancelCondition(ConditionBase condition, EntityBase affectedEntity)
        {
            if (_conditionDictionary.ContainsKey(condition))
            {
                var entityList = _conditionDictionary[condition];
                if (entityList.Contains(affectedEntity))
                {
                    condition.CancelCondition(affectedEntity);
                    return true;
                }
            }
            return false;
        }

        public static void CancelAllConditionOnEntity(EntityBase affectedEntity)
        {
            var list = (from pair in _conditionDictionary where pair.Value.Contains(affectedEntity) select pair.Key).ToList();
            list.ForEach(c => CancelCondition(c, affectedEntity));
        }

        public static ref ActionGroup<EntityBase> CancelMultiple(List<ConditionBase> conditions, EntityBase affectedEntity)
        {
            _cacheGroup?.Dispose();
            _cacheGroup = new ActionGroup<EntityBase>();
            foreach (var condition in conditions.Where(condition => CancelCondition(condition, affectedEntity))) _cacheGroup.AddAction(ref condition.UnModified);
            return ref _cacheGroup;
        }

        public static bool HasAny(List<ConditionBase> conditions, EntityBase entity)
        {
            return (from condition in conditions
                where _conditionDictionary.ContainsKey(condition) && _conditionDictionary[condition].Contains(entity)
                select condition).Any();
        }

        public static bool HasCondition(string type, EntityBase entity)
        {
            foreach (var condition in _conditionDictionary.Keys)
            {
                Debug.Log(condition.name);
                if (condition.name == type)
                {
                }
            }

            return true;
        }

        private static bool IsHighestPriority(ConditionBase condition, EntityBase entity)
        {
            return !(from pair in _conditionDictionary
                where pair.Key.priority >= condition.priority && pair.Value.Contains(entity) &&
                      pair.Key.GetType() == condition.GetType()
                select pair.Key).Any();
        }

        private static void CancelAllOfType(ConditionBase condition, EntityBase entity)
        {
            var conditionsAffectedEntity = _conditionDictionary.Where(c => c.Value.Contains(entity));
            var allConditionsOfType = conditionsAffectedEntity
                .Where(c => c.Key != condition && c.Key.GetType() == condition.GetType()).ToList();
            allConditionsOfType.ForEach(c => c.Key.CancelCondition(entity));
        }
    }
}