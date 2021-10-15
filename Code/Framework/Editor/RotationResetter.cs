// Primary Author : Viktor Dahlberg - vida6631

using UnityEditor;
using UnityEngine;
using static UnityEditor.Selection;

namespace Framework
{
	/// <summary>
	///     Editor extension to allow setting of transform euler rotations to world (0, 0, 0).
	/// </summary>
	public class RotationResetter : MonoBehaviour
    {
        [MenuItem("Extensions/Reset Selected Object Rotation")]
        private static void ResetRotation()
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                Debug.Log("0 objects changed");
                return;
            }

            foreach (var gameObject in gameObjects)
            {
                Undo.RegisterFullObjectHierarchyUndo(gameObject, "Reset Object Rotation");
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            Undo.FlushUndoRecordObjects();

            Debug.Log($"{gameObjects.Length} object(s) changed");
        }
    }
}