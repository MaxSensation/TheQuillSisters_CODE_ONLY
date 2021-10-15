// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using System.Collections.Generic;
using Entity.HealthSystem;
using Entity.Player;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class PoisonTrail : MonoBehaviour
    {
        [SerializeField]
        private float tickTime = default;
        [SerializeField]
        private float damage = default;
        [SerializeField]
        private float damageDistanceFromTrail = 0.4f;

        private Coroutine _coroutine;
        private PlayerHealth _health;
        private PlayerController _playerController;
        private TrailChecker _trailChecker;
        private TrailRenderer _trailRenderer;

        private void Start()
        {
            _trailChecker = transform.GetComponentInChildren<TrailChecker>();
            _playerController = FindObjectOfType<PlayerController>();
            _health = _playerController.GetComponent<PlayerHealth>();
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            var list = new Vector3[_trailRenderer.positionCount];
            _trailRenderer.GetPositions(list);
            list = CutDownList(list);

            if (_trailChecker.CheckPath(list) && Vector3.Distance(_trailRenderer.transform.parent.position, _playerController.transform.position) > damageDistanceFromTrail)
            {
                if (_coroutine == null)
                {
                    _coroutine = StartCoroutine(DamageOverTime());
                }
            }
            else
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
            }
        }

        private IEnumerator DamageOverTime()
        {
            while (true)
            {
                _health.TakeDamage(damage);
                yield return new WaitForSeconds(tickTime);
            }
        }

        private Vector3[] CutDownList(Vector3[] list)
        {
            var countdownPointList = new List<Vector3>();
            for (var index = 0; index < list.Length; index += 2)
            {
                countdownPointList.Add(list[index]);
            }
            return countdownPointList.ToArray();
        }
    }
}