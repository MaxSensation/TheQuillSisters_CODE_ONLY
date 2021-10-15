// Primary Author : Andreas Berzelius - anbe5918

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class ProjectileAim : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjRef<Vector3> playerPosition = default;

        private bool _hasReleasedProjectiles;

        private void Start()
        {
            Projectile.Ready += StopLookAt;
        }

        private void LateUpdate()
        {
            if (!_hasReleasedProjectiles)
            {
                gameObject.transform.LookAt(playerPosition.Value + Vector3.up);
            }
        }

        private void StopLookAt()
        {
            _hasReleasedProjectiles = true;
            Projectile.Ready -= StopLookAt;
        }
    }
}