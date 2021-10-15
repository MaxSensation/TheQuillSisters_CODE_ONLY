// Primary Author : Andreas Berzelius - anbe5918

using System;
using Combat.Interfaces;
using Entity.AI.States.Behavior;
using Environment;
using UnityEngine;

namespace Entity.AI.Scarabs
{
    public class Scarab : Enemy, IKnockable, ISpawnable
    {
        [SerializeField]
        private GameObject healthOrbPrefab = default;
        [HideInInspector]
        public Scarab leaderObj = default;
        [HideInInspector]
        public Vector3 targetPosition = default;
        [HideInInspector]
        public ScarabFlock flock = default;

        public bool isLeader;
        private SphereCollider _collider;

        private new void Start()
        {
            _collider = GetComponent<SphereCollider>();
            base.Start();
            OnAttack += TriggerFlock;
            if (isLeader)
            {
                OnDied += SpawnHealthOrb;
                leaderObj = this;
            }
        }

        private void OnDisable()
        {
            if (!isLeader && !(leaderObj.BehaviorStateMachine.CurrentState is Hunting))
            {
                leaderObj.BehaviorStateMachine.TransitionTo<Hunting>();
            }
            OnDied -= SpawnHealthOrb;
            if (flock != null)
            {
                flock.ScarabDied();
            }
        }

        public Vector3 GetSpawnScale()
        {
            return Vector3.one * 0.4f;
        }

        private void TriggerFlock(GameObject obj)
        {
            var scarab = obj.GetComponent<Scarab>();
            if (scarab && scarab.leaderObj == leaderObj && scarab != this && !(BehaviorStateMachine.CurrentState is Hunting))
            {
                BehaviorStateMachine.TransitionTo<Hunting>();
            }
        }

        private void SpawnHealthOrb(GameObject obj)
        {
            if (obj == gameObject)
            {
                Instantiate(healthOrbPrefab, transform.position, Quaternion.identity, gameObject.transform.parent);
            }
        }

        public override Tuple<float, float> GetDimensions()
        {
            return new Tuple<float, float>(_collider.radius, _collider.radius * 2);
        }
    }
}