using Cinemachine;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.Cam
{
    public class VirtualCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vcam;

        public static VirtualCamera New(Transform root, Transform lookPivot)
        {
            var newGO = Instantiate(GlobalSettingsLoader.Load().Camera.VirtualCameraPrefab, root);
            var vcamScript = newGO.GetComponent<VirtualCamera>();
            vcamScript.Initialize(lookPivot);
            return vcamScript;
        }

        private void Initialize(Transform lookPivot)
        {
            vcam.Follow = lookPivot;
            transform.position = lookPivot.transform.position;
            SessionSettings.Instance.CamRotation = vcam.transform.rotation;
        }
    }
}
