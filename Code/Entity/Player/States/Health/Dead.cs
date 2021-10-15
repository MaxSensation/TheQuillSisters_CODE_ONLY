// Primary Author : Erik Pilström - erpi3245

using Entity.Player.States.Physical;
using Framework.SaveSystem;
using Framework.ScriptableObjectEvent;
using UI;
using UnityEngine;

namespace Player.States.Health
{
    [CreateAssetMenu(menuName = "States/PlayerStates/HealthStates/Dead")]
    public class Dead : PlayerBaseState
    {
        [SerializeField] 
        private GameEvent readyToRespawnPlayer = default;

        public override void Enter()
        {
            readyToRespawnPlayer.OnEvent += RespawnPlayer;
            LoadingScreen.GameLoadRequested?.Invoke(!SaveManager.HasSave());
        }

        public override void Exit()
        {
            readyToRespawnPlayer.OnEvent -= RespawnPlayer;
        }

        public override void Run()
        {
        }

        private void RespawnPlayer()
        {
            Player.HealthStateMachine.TransitionTo<Alive>();
        }
    }
}