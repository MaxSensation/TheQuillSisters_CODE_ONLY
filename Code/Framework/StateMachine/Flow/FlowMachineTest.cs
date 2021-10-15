// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.StateMachine.Flow
{
    public class FlowMachineTest : MonoBehaviour
    {
        [SerializeField] 
        private FlowState lightAttack = default;
        [SerializeField] 
        private FlowState heavyAttack = default;

        private bool _active;
        private FlowMachine _flowMachine;

        private void Update()
        {
            if (_active)
            {
                _flowMachine.Run();
            }
        }

        private void BeginLightAttackComboForWhateverReason()
        {
            var comboCounter = 0;
            if (_flowMachine is null)
            {
                _flowMachine = new FlowMachine(lightAttack, comboCounter);
                _active = true;
            }
            else
            {
                _flowMachine.Goto(lightAttack, comboCounter);
            }
        }

        private void BeginHeavy()
        {
            _flowMachine.Goto(heavyAttack);
        }
    }
}