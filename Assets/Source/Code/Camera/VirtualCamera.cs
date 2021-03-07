using Cinemachine;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.Cam
{
    public class VirtualCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private CinemachineConfiner confiner;
        [SerializeField] private GameObject cameraBordersPrefab;

        private Transform lookPivot, camBorders;
        private SessionSettings sessionSettings;

        public static VirtualCamera New(Transform root, Transform lookPivot)
        {
            var newGO = Instantiate(GlobalSettingsLoader.Load().Camera.VirtualCameraPrefab, root);
            var vcamScript = newGO.GetComponent<VirtualCamera>();
            vcamScript.Initialize(lookPivot);
            return vcamScript;
        }

        private void Initialize(Transform lookPivot)
        {
            this.lookPivot = lookPivot;
            sessionSettings = SessionSettings.Instance;
            vcam.Follow = lookPivot;
            transform.position = lookPivot.transform.position;
            sessionSettings.CamRotation = vcam.transform.rotation;

            //camera borders
            camBorders = Instantiate(cameraBordersPrefab).transform;
            confiner.m_BoundingVolume = camBorders.GetComponent<Collider>();
            if (confiner.m_BoundingVolume == null) 
                Debug.LogError("confiner.m_BoundingVolume is null because cameraBordersPrefab doesn't contain Collider component", camBorders);
            SetConfinerActive(false);

            sessionSettings.LocalState.StateChanged += OnLocalStateChanged;
        }

        private void OnLocalStateChanged(LocalState.LocalStates localState)
        {
            switch (localState)
            {
                case LocalState.LocalStates.Player:
                    {
                        SetConfinerActive(true);
                    }
                    break;
                case LocalState.LocalStates.Spectator:
                    {
                        SetConfinerActive(false);
                    }
                    break;
            }
        }

        private void SetConfinerActive(bool isActive)
        {
            camBorders.parent = isActive ? sessionSettings.ControlledUnit.Transform : lookPivot;
            camBorders.localPosition = isActive ? new Vector3(0, 0, -6.88f) : Vector3.zero;
            camBorders.localRotation = Quaternion.identity;
            confiner.enabled = isActive;
            camBorders.gameObject.SetActive(isActive);
        }
    }
}
