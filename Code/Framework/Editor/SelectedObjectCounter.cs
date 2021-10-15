// Primary Author : Viktor Dahlberg - vida6631

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
	/// <summary>
	///     Editor extension to allow counting of gameObjects.
	/// </summary>
	public class SelectedObjectCounter : MonoBehaviour
    {
        [MenuItem("Extensions/Log Selected Object Count")]
        private static void LogSelectedObjectCount()
        {
            Debug.Log($"Selected objects: {Selection.objects.Length}");
            Debug.Log($"Active objects: {(from obj in Selection.objects where obj is GameObject go && go.activeInHierarchy select obj).Count()}");
        }
    }
}