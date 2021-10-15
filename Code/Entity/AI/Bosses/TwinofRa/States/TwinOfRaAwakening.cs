// Primary Author : Maximiliam Rosén - maka4519

using Entity.AI.States;
using UnityEngine;

namespace Entity.AI.Bosses.TwinofRa.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/TwinOfRa/Awakening")]
    public class TwinOfRaAwakening : AIBaseState
    {
        public override void Enter()
        {
            base.Enter();
            StateMachine.TransitionTo<TwinOfRaAttacking>();
        }
    }
}