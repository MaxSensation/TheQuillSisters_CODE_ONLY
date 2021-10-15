// Primary Author : Maximiliam Rosén - maka4519

#if DEBUG
using Entity.AI.Bosses;
using Entity.HealthSystem;
using Environment.Door;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugModes
{
    public class DebugMode : MonoBehaviour
    {
        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard.kKey.wasPressedThisFrame)
            {
                var enemies = FindObjectsOfType<EnemyHealth>();
                foreach (var enemy in enemies)
                {
                    if (!enemy.GetComponent<Boss>())
                    {
                        enemy.TakeDamage(int.MaxValue);
                    }
                }
            }

            if (keyboard.oKey.wasPressedThisFrame)
            {
                var enemies = FindObjectsOfType<EnemyHealth>();
                foreach (var enemy in enemies)
                {
                    if (enemy.GetComponent<Boss>())
                    {
                        enemy.TakeDamage(int.MaxValue);
                    }
                }
            }

            if (keyboard.lKey.wasPressedThisFrame)
            {
                FindObjectOfType<PlayerHealth>().TakeDamage(int.MaxValue);
            }

            if (keyboard.pKey.wasPressedThisFrame)
            {
                FindObjectOfType<Door>().ChangeState(true);
            }
        }
    }
}

#endif