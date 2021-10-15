// Primary Author : Andreas Berzelius - anbe5918
// Secondary Author : Erik Pilström - erpi3245

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Alerted")]
    public class Alerted : AIBaseState
    {
        [SerializeField] 
        private ScriptObjVar<float> alertedTime = default;

        private float _time;

        public override void Enter()
        {
            AI.agent.SetDestination(AI.playerPosition.Value);
            _time = 0;
        }

        public override void Run()
        {
            base.Run();
            //return to origin position if PATROLLER and resume patrolling
            //if not PATROLLER, go to idling
            if (AI.agent.velocity.magnitude < 0.1f)
            {
                _time += Time.deltaTime;
            }

            if (_time > alertedTime.value)
            {
                Debug.Log("returning to idling");
                StateMachine.TransitionTo<Idling>();
            }

            if (AI.CanSeePlayer())
            {
                StateMachine.TransitionTo<Hunting>();
            }
        }
    }
}