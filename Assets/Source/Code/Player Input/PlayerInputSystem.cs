using Source.Code.Units;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.PlayerInput
{
    public class PlayerInputSystem : MonoBehaviour
    {
        private Vector2 maxDistFromUnit;
        private GlobalSettings globalSettings;
        private SessionSettings sessionSettings;
        private Unit controlledUnit;

        public LookPivot LookPivot { get; private set; }

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
            globalSettings = GlobalSettingsLoader.Load();
            this.sessionSettings = playerSettings;
            maxDistFromUnit = globalSettings.Input.MaxLookPivotDistance;
            LookPivot = LookPivot.New(transform);
        }
        #endregion

        private void Start()
        {
            controlledUnit = SessionSettings.Instance.ControlledUnit;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) controlledUnit.AttackComponent.TryRaiseMainAttack();
            LookPivotMove();
            HandleKeyboardInput();
        }

        private void HandleKeyboardInput()
        {
            var hor = Input.GetAxisRaw("Horizontal");
            var vert = Input.GetAxisRaw("Vertical");

            Vector2 deltaMove = new Vector2(hor, vert);
            controlledUnit.DeltaMove = deltaMove;

            controlledUnit.MoverComponent.Move(deltaMove);
            controlledUnit.MoverComponent.LookAt(LookPivot.Transform);
        }

        private void LookPivotMove()
        {
            var mousePos = Input.mousePosition;

            int screenHeight = Screen.height;
            int screenWidth = Screen.width;

            float widthDeltaPos = (Mathf.Clamp01(mousePos.x / screenWidth) - 0.5f) * 2;
            float heightDeltaPos = (Mathf.Clamp01(mousePos.y / screenHeight) - 0.5f) * 2;

            Vector3 pos = controlledUnit.Transform.position + Vector3.forward * heightDeltaPos * maxDistFromUnit.y + Vector3.right * widthDeltaPos * maxDistFromUnit.x;

            LookPivot.SetPosition(pos);
        }
    }
}