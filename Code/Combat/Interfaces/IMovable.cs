// Primary Author : Maximiliam Rosén - maka4519

using Combat.ConditionSystem;
using UnityEngine;

namespace Combat.Interfaces
{
    public interface IMovable
    {
        void MoveToPosition(Vector3 movePosition);
        void OnMoveStarted(ConditionBase condition);
        void OnMoveCompleted(ConditionBase condition);
    }
}