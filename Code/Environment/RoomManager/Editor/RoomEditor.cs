// Primary Author : Maximiliam Rosén - maka4519

using UnityEditor;
using UnityEngine;

namespace Environment.RoomManager
{
    /// <summary>
    ///     Editor Script for adding a Round to the Room gameobject
    /// </summary>
    [CustomEditor(typeof(Room))]
    public class RoomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var instance = (Room) target;
            if (GUILayout.Button("Add Round"))
            {
                instance.AddRound();
            }

            DrawDefaultInspector();
        }
    }
}