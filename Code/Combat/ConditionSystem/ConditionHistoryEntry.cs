// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Combat.ConditionSystem
{
    public struct ConditionHistoryEntry
    {
        public ConditionBase Condition;
        public EntityBase ApplierEntity;
        public EntityBase AffectedEntity;
        public float Timestamp;
        public ConditionEntryType Type;

        public void Print()
        {
            Debug.Log(
                $"Condition: {Condition?.name} " +
                $"ApplierEntity: {ApplierEntity?.name} " +
                $"AffectedEntity: {AffectedEntity?.name} " +
                $"Timestamp: {Timestamp} " +
                $"Type: {Type}"
            );
        }
    }

    public enum ConditionEntryType
    {
        Modified,
        Updated,
        UnModify
    }
}