// Primary Author : Maximiliam Rosén - maka4519

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Environment.RoomManager
{
    /// <summary>
    ///     Records the editors mouse position and moves the SpawnPoint to clicked area.
    /// </summary>
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        private SpawnPoint _instance;
        private bool _isInitialized;
        private bool _isRecording;
        private Vector3 _oldPos;

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!_isRecording)
            {
                return;
            }
            if (_instance == null)
            {
                _isRecording = false;
                SceneView.duringSceneGui -= OnSceneGUI;
            }
            else
            {
                if (Selection.activeGameObject != _instance.gameObject)
                {
                    _instance.SetSpawnPosition(_oldPos);
                    _isRecording = false;
                    SceneView.duringSceneGui -= OnSceneGUI;
                }

                // Mouse to screen and raycast to position
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
                var ray = sceneView.camera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
                {
                    if (hitInfo.collider)
                    {
                        _instance.SetSpawnPosition(hitInfo.point + Vector3.up * 0.1f);
                    }

                    if (Mouse.current.leftButton.isPressed)
                    {
                        _isRecording = false;
                        SceneView.duringSceneGui -= OnSceneGUI;
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            _instance = (SpawnPoint) target;
            _isInitialized = _instance.GetStatus();
            // Check if the instance of the SpawnPoint has been initialized if so then record mouse position
            if (!_isInitialized)
            {
                _instance.SetInit();
                _isRecording = true;
                SceneView.duringSceneGui += OnSceneGUI;
                _oldPos = _instance.transform.position;
            }

            // If started recording then start mouse tracking and move object in edit mode
            if (GUILayout.Button("Record") && !_isRecording)
            {
                _isRecording = true;
                SceneView.duringSceneGui += OnSceneGUI;
                _oldPos = _instance.transform.position;
            }

            DrawDefaultInspector();
        }
    }
}