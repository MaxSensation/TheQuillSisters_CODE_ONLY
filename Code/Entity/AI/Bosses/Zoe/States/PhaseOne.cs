// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe5918

using Entity.HealthSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Zoe/PhaseOne")]
    public class PhaseOne : ZoeBaseState
    {
        private EnemyHealth _health;
        
        public override void Enter()
        {
            _health = Boss.gameObject.GetComponent<EnemyHealth>();
            _health.SetInvincibility(true);
        }
        
        public override void Run()
        {
            if (Boss.localAttacks[0].GetTimeLeft() <= 0)
            {
                Boss.localAttacks[0].Trigger();
            }
            LookAtPlayer();
        }

        public override void Exit()
        {
            _health.SetInvincibility(false);
        }
    }
}