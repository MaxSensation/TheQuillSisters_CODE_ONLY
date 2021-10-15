// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author: Viktor Dahlberg - vida6631

using System;
using UnityEngine;

namespace Entity.AI.Bosses
{
    public abstract class Boss : Enemy
    {
        public static Action OnBossDied;

        [SerializeField] 
        protected string displayName = default;
    }
}