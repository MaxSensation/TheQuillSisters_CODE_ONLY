// Primary Author : Maximiliam Rosén - maka4519

using UnityEditor;
using UnityEngine;

namespace Environment.RoomManager
{
    /// <summary>
    ///     Editor Script for adding a SpawnPoint to the Round gameobject
    /// </summary>
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var instance = (Spawner) target;
            if (GUILayout.Button("Add SpawnPoint"))
            {
                instance.AddSpawnPoint();
            }

            DrawDefaultInspector();
        }
    }
}