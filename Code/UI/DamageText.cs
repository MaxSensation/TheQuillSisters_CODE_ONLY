// Primary Author : Andreas Berzelius - anbe4918

using UnityEngine;

namespace UI
{
    public class DamageText : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 1f);
            transform.localPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f), Random.Range(-0.1f, 0.5f));
        }
    }
}