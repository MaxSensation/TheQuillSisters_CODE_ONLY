// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Entity.AI.Mummies
{
    public class MummyGiant : Mummy
    {
        public override Vector3 GetSpawnScale()
        {
            return Vector3.one * 2f;
        }
    }
}