// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using System.Collections.Generic;
using Combat.AttackCollider;
using Entity.AI;
using Entity.HealthSystem;
using UnityEngine;

namespace Framework.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class EntityAudioManager : MonoBehaviour
    {
        [SerializeField] 
        private GameObject entityGameObject = default;
        [SerializeField] 
        private AttackColliderCollisionDetection attackColliderCollisionDetection = default;
        [SerializeField] 
        private AttackAudio[] attackSounds = default;
        [SerializeField] 
        private SoundBundle tookDamageSound = default;
        [SerializeField] 
        private SoundBundle dieSound = default;

        private readonly Dictionary<string, AttackAudio> _audioLibrary = new Dictionary<string, AttackAudio>();
        private AudioSource _audioSource;
        private string _currentAttack;

        private void Start()
        {
            var parent = transform.parent;
            _audioSource = parent.GetComponentInChildren<AudioSource>();
            attackColliderCollisionDetection.OnAttackHit += OnHit;
            Health.TookDamage += (o, _) => TookDamage(o);
            Health.EntityDied += OnDied;
            StartCoroutine(StartWithDelay());
        }

        private IEnumerator StartWithDelay()
        {
            yield return new WaitForEndOfFrame();
            CreateDictionary();
        }

        private void OnDied(GameObject o)
        {
            if (dieSound && entityGameObject == o)
            {
                _audioSource.PlayOneShot(tookDamageSound.GetRandom());
            }
        }

        private void TookDamage(GameObject o)
        {
            if (tookDamageSound && entityGameObject == o)
            {
                _audioSource.PlayOneShot(tookDamageSound.GetRandom());
            }
        }

        private void CreateDictionary()
        {
            var enemy = entityGameObject.GetComponent<Enemy>();
            foreach (var attackBundle in attackSounds)
                if (enemy)
                {
                    var enemyAttack = enemy.localAttacks.Find(a => a.GetTitle() == attackBundle.attack.GetTitle());
                    if (attackBundle.immediateBegin)
                    {
                        enemyAttack.OnImmediateBegin += () => OnAttack(attackBundle.attack.GetTitle());
                    }
                    else
                    {
                        enemyAttack.OnBeginAttack += () => OnAttack(attackBundle.attack.GetTitle());
                    }

                    enemyAttack.OnSpecial += () => OnSpecial(attackBundle.attack.GetTitle());
                    _audioLibrary.Add(enemyAttack.GetTitle(), attackBundle);
                }
                else
                {
                    if (attackBundle.immediateBegin)
                    {
                        attackBundle.attack.OnImmediateBegin += () => OnAttack(attackBundle.attack.GetTitle());
                    }
                    else
                    {
                        attackBundle.attack.OnBeginAttack += () => OnAttack(attackBundle.attack.GetTitle());
                    }

                    attackBundle.attack.OnSpecial += () => OnSpecial(attackBundle.attack.GetTitle());
                    _audioLibrary.Add(attackBundle.attack.GetTitle(), attackBundle);
                }
        }

        private void OnAttack(string title)
        {
            _currentAttack = title;
            if (_audioLibrary.ContainsKey(_currentAttack))
            {
                _audioSource.PlayOneShot(_audioLibrary[_currentAttack].beginAttack);
            }
        }

        private void OnHit()
        {
            if (_audioLibrary.ContainsKey(_currentAttack))
            {
                _audioSource.PlayOneShot(_audioLibrary[_currentAttack].hitAttack);
            }
        }

        private void OnSpecial(string title)
        {
            _currentAttack = title;
            if (_audioLibrary.ContainsKey(_currentAttack))
            {
                _audioSource.PlayOneShot(_audioLibrary[_currentAttack].specialAttack);
            }
        }
    }
}