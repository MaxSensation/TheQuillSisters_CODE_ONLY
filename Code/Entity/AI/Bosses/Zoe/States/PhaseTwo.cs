// Primary Author : Andreas Berzelius - anbe5918

using UnityEngine;

namespace Entity.AI.Bosses.Zoe.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Zoe/PhaseTwo")]
    public class PhaseTwo : ZoeBaseState
    {
        [SerializeField] 
        private float attackRange = 5f;

        public override void Run()
        {
            base.Run();
            var distanceMagnitude = (Boss.playerPosition.Value - Boss.transform.position).magnitude;
            Boss.agent.SetDestination(Boss.playerPosition.Value);
            if (distanceMagnitude < attackRange && Boss.localAttacks[1].GetTimeLeft() <= 0)
            {
                Boss.localAttacks[1].Trigger();
            }

            LookAtPlayer();
        }
    }
}