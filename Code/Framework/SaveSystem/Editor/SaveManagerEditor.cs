// Primary Author : Viktor Dahlberg - vida6631

using UnityEditor;
using UnityEngine;

namespace Framework.SaveSystem
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var instance = (SaveManager) target;
            if (GUILayout.Button("Generate Default Save"))
            {
                instance.GenerateDefaultSave();
            }

            if (GUILayout.Button("Load Default Save"))
            {
                instance.LoadDefaultSave();
            }

            if (GUILayout.Button("Delete Save"))
            {
                instance.DeleteSave();
            }

            DrawDefaultInspector();
        }
    }
}