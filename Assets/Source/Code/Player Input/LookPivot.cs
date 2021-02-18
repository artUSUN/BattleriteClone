using Source.Code.Cam;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.PlayerInput
{
    public class LookPivot : MonoBehaviour
    {
        private Vector2 maxDistFromUnit;
        private SessionSettings sessionSettings;
        private GlobalSettings settings;

        public Transform Transform { get; private set; }

        #region Сonstructor
        public static LookPivot New(Transform parent)
        {
            var newGO = Instantiate(GlobalSettingsLoader.Load().Input.LookPivotPrefab, parent);
            var lookPivot = newGO.GetComponent<LookPivot>();
            lookPivot.Initialize();
            return lookPivot;
        }

        private void Initialize()
        {
            Transform = transform;
            sessionSettings = SessionSettings.Instance;
            settings = GlobalSettingsLoader.Load();
            maxDistFromUnit = settings.Input.MaxLookPivotDistance;
        }
        #endregion

        public void SetPosition(Vector3 mousePos)
        {
            if (sessionSettings.ControlledUnit == null) FreeCam(mousePos);
            else PlayerSightCam(mousePos);
        }

        private void FreeCam(Vector3 mousePos)
        {
            Ray ray = MainCamera.Instance.Camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hitInfo, 100f, settings.Layers.Ground);
            if (hitInfo.transform != null) Transform.position = hitInfo.point;
        }

        private void PlayerSightCam(Vector3 mousePos)
        {
            int screenHeight = Screen.height;
            int screenWidth = Screen.width;

            float widthDeltaPos = (Mathf.Clamp01(mousePos.x / screenWidth) - 0.5f) * 2;
            float heightDeltaPos = (Mathf.Clamp01(mousePos.y / screenHeight) - 0.5f) * 2;

            Vector3 offset = Vector3.forward * heightDeltaPos * maxDistFromUnit.y + Vector3.right * widthDeltaPos * maxDistFromUnit.x;

            Transform.position = sessionSettings.ControlledUnit.Transform.position + offset;
        }
    }
}