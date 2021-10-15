// Primary Author : Maximiliam Rosén - maka4519

using Entity.Player;
using UnityEngine;

namespace Combat.Entity.AI
{
    public class HeadRemover : MonoBehaviour
    {
        [SerializeField] 
        private float delay = default;

        private float _time;

        private void OnTriggerEnter(Collider other)
        {
            _time = 0f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_time >= delay)
            {
                var enemyAvoidance = other.GetComponent<EnemyAvoidance>();
                if (enemyAvoidance) enemyAvoidance.ActivateObstacle();
            }

            _time += Time.deltaTime;
        }
    }
}