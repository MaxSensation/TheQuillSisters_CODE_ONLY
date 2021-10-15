// Primary Author : Andreas Berzelius - anbe4918

using Entity.HealthSystem;
using UI.Bar;
using UnityEngine;

namespace Entity.AI
{
    public class EnemyCanvas : MonoBehaviour
    {
        [SerializeField]
        private HealthBar healthBar = default;
        [SerializeField]
        private GameObject damageTextPrefab = default;

        private Transform _cameraPos;
        private Canvas _canvas;
        private Animation _damageTextAnimation;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            healthBar.SetHealthComponentTarget(GetComponentInParent<EnemyHealth>());
            _cameraPos = Camera.main.transform;
            Health.TookDamage += ReactToDamage;
        }

        private void LateUpdate()
        {
            transform.LookAt(_cameraPos);
        }

        private void OnDestroy()
        {
            Health.TookDamage -= ReactToDamage;
        }

        private void ReactToDamage(GameObject attackedEnemy, float damage)
        {
            if (attackedEnemy.Equals(gameObject.transform.parent.gameObject))
            {
                UpdateHealthBar(damage);
            }
        }

        private void UpdateHealthBar(float damage)
        {
            if (!_canvas.isActiveAndEnabled)
            {
                _canvas.enabled = true;
            }

            //damageText.color = Color.white;
            //damageText.transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(Randomintenzity.y, 1f),Random.Range(-0.1f, 0.5f));
            var dt = Instantiate(damageTextPrefab, transform);
            //dt.GetComponent<Animation>().Play();
            //_damageTextAnimation.Play();
            dt.GetComponent<TextMesh>().text = damage.ToString();
            //damageText.text = damage.ToString();
        }
    }
}