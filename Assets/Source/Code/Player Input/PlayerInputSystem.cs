using Source.Code.Utils;
using System;
using UnityEngine;

namespace Source.Code.PlayerInput
{
    public class PlayerInputSystem : MonoBehaviour
    {
        private Vector3 lastFrameMousePos;

        public LookPivot LookPivot { get; private set; }

        public event Action SpacePressed;
        public event Action Mouse0Pressed;
        public event Action<Vector2> DeltaMove;

        #region Constructor
        public static PlayerInputSystem New(Transform parent, SessionSettings settings)
        {
            var newEmpty = new GameObject("Player Input Handler");
            newEmpty.transform.SetParent(parent);
            var createdSystem = newEmpty.AddComponent<PlayerInputSystem>();
            createdSystem.Initialize(settings);
            return createdSystem;
        }

        private void Initialize(SessionSettings playerSettings)
        {
            LookPivot = LookPivot.New(transform);
        }
        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) Mouse0Pressed?.Invoke();
            if (Input.GetKeyDown(KeyCode.Space)) SpacePressed?.Invoke();
            LookPivotMove();
            HandleKeyboardInput();
        }

        public void UnsubscribeEvents()
        {
            SpacePressed = null;
            Mouse0Pressed = null;
            DeltaMove = null;
        }

        private void HandleKeyboardInput()
        {
            var hor = Input.GetAxisRaw("Horizontal");
            var vert = Input.GetAxisRaw("Vertical");

            Vector2 deltaMove = new Vector2(hor, vert);
            DeltaMove?.Invoke(deltaMove);

            //controlledUnit.DeltaMove = deltaMove;
            //controlledUnit.MoverComponent.Move(deltaMove);
            //controlledUnit.MoverComponent.LookAt(LookPivot.Transform);
        }

        private void LookPivotMove()
        {
            var mousePosition = Input.mousePosition;
            if (lastFrameMousePos != mousePosition)
            {
                LookPivot.SetPosition(mousePosition);
            }
            lastFrameMousePos = mousePosition;
        }
    }
}