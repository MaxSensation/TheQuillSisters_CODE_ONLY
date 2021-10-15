// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using Environment.RoomManager;
using Environment.Trigger;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Door
{
	/// <summary>
	///     This class open and closes doors
	/// </summary>
	public class Door : MonoBehaviour
    {
        [SerializeField]
        private bool startedOpened = default;
        [SerializeField]
        private ScriptObjVar<bool> eyeOfHorus = default;
        [SerializeField]
        private Room room = default;
        [SerializeField]
        private List<AreaTrigger> areaTriggers = new List<AreaTrigger>();
        [SerializeField]
        private GameEventGroup setState = default;
        [SerializeField]
        private GameObject lockProp = default;
        [SerializeField]
        private ScriptObjVar<Vector3> playerPosition = default;
        [SerializeField]
        private float playerActivationDistance = default;

        public static Action onKeyOpenDoor;
        private Animator _animator;
        private bool _isKeyDoor;
        private bool _originalState;
        private bool _pendingOpening;

        private BoxCollider _wall;

        private void Start()
        {
            _wall = GetComponent<BoxCollider>();
            if (room)
            {
                _isKeyDoor = room.HasKeys();
            }

            if (_isKeyDoor)
            {
                lockProp.SetActive(true);
            }

            setState.OnEvent += ForceState;
            _originalState = startedOpened;
            _animator = GetComponentInChildren<Animator>();
            if (startedOpened)
            {
                ChangeState(true);
                _animator.Play("Open", 0, 10f);
            }
            
            if (room != null)
            {
                room.OnRoomCompleted += () => ChangeState(true);
            }

            if (areaTriggers != null)
            {
                foreach (var trigger in areaTriggers)
                {
                    trigger.OnTrigger += ChangeState;
                }
            }
        }

        private void Update()
        {
            if (!_pendingOpening)
            {
                return;
            }

            if ((transform.position - playerPosition.value).magnitude < playerActivationDistance)
            {
                _pendingOpening = false;
                _animator.SetBool("open", true);
                _wall.enabled = false;
                lockProp.SetActive(false);
                onKeyOpenDoor?.Invoke();
            }
        }

        private void OnDestroy()
        {
            setState.OnEvent -= ForceState;
        }

        private void ForceState()
        {
            if (eyeOfHorus != null && eyeOfHorus)
            {
                startedOpened = !_originalState;
                ChangeState(startedOpened);
                _animator.Play(startedOpened ? "Open" : "Close", 0, 10f);
            }
            else
            {
                ChangeState(_originalState);
                _animator.Play(_originalState ? "Open" : "Close", 0, 10f);
            }
        }

        /// <summary>
        ///     Change the state of the door.
        /// </summary>
        /// <param name="open">The new state that door will change too.</param>
        public void ChangeState(bool open)
        {
            if (_isKeyDoor && open)
            {
                _pendingOpening = true;
            }
            else
            {
                _wall.enabled = !open;
                _animator.SetBool("open", open);
            }
        }
    }
}