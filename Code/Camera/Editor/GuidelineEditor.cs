// Primary Author : Viktor Dahlberg - vida6631

using UnityEditor;
using UnityEngine;

namespace Scripts.Camera
{
    [CustomEditor(typeof(Guideline))]
    public class GuidelineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var instance = (Guideline) target;
            if (GUILayout.Button("Add Guidepoint"))
            {
                instance.AddGuidePoint();
                EditorUtility.SetDirty(target);
            }

            DrawDefaultInspector();
        }
    }
}