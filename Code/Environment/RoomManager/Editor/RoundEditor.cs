// Primary Author : Maximiliam Rosén - maka4519

using UnityEditor;
using UnityEngine;

namespace Environment.RoomManager
{
    /// <summary>
    ///     Editor Script for adding a SpawnPoint to the Round gameobject
    /// </summary>
    [CustomEditor(typeof(Round))]
    public class RoundEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var instance = (Round) target;
            if (GUILayout.Button("Add SpawnPoint"))
            {
                instance.AddSpawnPoint();
            }

            DrawDefaultInspector();
        }
    }
}