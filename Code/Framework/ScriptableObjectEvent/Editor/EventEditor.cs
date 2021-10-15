// Primary Author : Maximiliam Rosén - maka4519

using UnityEditor;
using UnityEngine;

namespace Framework.ScriptableObjectEvent
{
    [CustomEditor(typeof(GameEvent), true)]
    public class EventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = Application.isPlaying;
            var e = target as GameEvent;
            if (GUILayout.Button("Raise"))
            {
                e?.Raise();
            }
        }
    }
}