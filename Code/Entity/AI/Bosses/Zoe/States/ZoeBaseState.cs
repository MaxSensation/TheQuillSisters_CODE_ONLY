// Primary Author : Maximiliam Rosén - maka4519

using Entity.AI.States;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.States
{
    public class ZoeBaseState : AIBaseState
    {
        [SerializeField] [Tooltip("How fast it should rotate at the player")] [Range(1f, 10f)]
        private float rotationSpeed;

        protected BossZoe Boss => AI as BossZoe;

        public override void Run()
        {
            if (!Boss.agent)
            {
                return;
            }

            if (Boss.agent.velocity.magnitude > 0.1f && !Boss.IsWalking())
            {
                Boss.StartWalkAnimation();
                Boss.zoeAudioSource.Play();
            }
            else if (AI.agent.velocity.magnitude < 0.001f && Boss.IsWalking())
            {
                Boss.StopWalkAnimation();
                Boss.zoeAudioSource.Stop();
            }
        }

        protected void LookAtPlayer()
        {
            var newRotation = Quaternion.LookRotation(Boss.playerPosition.Value - Boss.transform.position, Vector3.up);
            newRotation.x = 0.0f;
            newRotation.z = 0.0f;
            Boss.transform.rotation = Quaternion.Slerp(Boss.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }
    }
}