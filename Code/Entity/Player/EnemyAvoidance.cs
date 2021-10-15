// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Player
{
    [RequireComponent(typeof(NavMeshObstacle))]
    public class EnemyAvoidance : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjVar<bool> isGrounded = default;

        private NavMeshObstacle _obstacle;

        private void Start()
        {
            _obstacle = GetComponent<NavMeshObstacle>();
        }

        private void Update()
        {
            if (isGrounded)
            {
                DeactivateObstacle();
            }
        }

        public void ActivateObstacle()
        {
            _obstacle.enabled = true;
        }

        private void DeactivateObstacle()
        {
            _obstacle.enabled = false;
        }
    }
}