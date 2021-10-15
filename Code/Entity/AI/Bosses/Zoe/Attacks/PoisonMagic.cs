// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe5918

using Combat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Bosses/Zoe/PoisonMagic")]
    public class PoisonMagic : ScriptObjAttackBase
    {
        [SerializeField]
        private GameObject poisonMagicGO = default;
        [SerializeField]
        private Vector3 offSetVector = Vector3.up;
        [SerializeField]
        private float offSetMultiplier = 0.9f;

        protected override void Attack()
        {
            var poisonGO = Instantiate(
                poisonMagicGO, 
                owner.transform.position + offSetVector * offSetMultiplier, 
                Quaternion.identity);
            SceneManager.MoveGameObjectToScene(poisonGO, owner.scene);
        }
    }
}