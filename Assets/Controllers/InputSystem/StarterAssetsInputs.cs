using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Debug Settings")]
		public InputAction activateDebug;
        public bool debugMode = false;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

#if DEBUG
		private void Awake()
        {
			activateDebug.performed += OnActivateDebug;
			activateDebug.Enable();
        }
        private void OnDestroy()
        {
            activateDebug.performed -= OnActivateDebug;
            activateDebug.Disable();
        }
        public void OnActivateDebug(InputAction.CallbackContext ctx)
		{
			if(debugMode)
			{
				debugMode = false;
				SetCursorState(true, true);
            }
            else
			{
				debugMode = true;
                SetCursorState(false,false);
            }
        }
#endif
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked, true);
		}

		private void SetCursorState(bool newState, bool cursorInputForLook)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
			this.cursorInputForLook = cursorInputForLook;
		}
	}
	
}