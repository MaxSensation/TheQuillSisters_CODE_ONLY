// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Player.MovementAbilities
{
    public class MovementAbility : MonoBehaviour
    {
        [Header("Movement Ability")] [SerializeField]
        protected float cooldownMilli;
        protected PlayerMovement Movement;
        protected bool notInCoolDown = true;
        protected PlayerController PlayerController;

        protected virtual void Start()
        {
            Movement = GetComponent<PlayerMovement>();
            PlayerController = GetComponent<PlayerController>();
        }

        protected async void StartCooldown()
        {
            notInCoolDown = false;
            await Task.Delay(TimeSpan.FromMilliseconds(cooldownMilli));
            notInCoolDown = true;
        }
    }
}