// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class EntityRendererViewer : MonoBehaviour
    {
        [SerializeField] private EntityBase entity = default;
        
        public static List<EntityBase> Entities = new List<EntityBase>();
        
        private void OnBecameInvisible()
        {
            if (Entities.Contains(entity))
            {
                Entities.Remove(entity);
            }
        }

        private void OnBecameVisible()
        {
            if (!Entities.Contains(entity))
            {
                Entities.Add(entity);
            }
        }
    }
}