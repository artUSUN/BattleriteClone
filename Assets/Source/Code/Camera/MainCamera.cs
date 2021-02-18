using UnityEngine;

namespace Source.Code.Cam
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        public static MainCamera Instance { get; private set; }

        public Transform Transform { get; private set; }
        public Camera Camera { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Transform = transform;
            Camera = GetComponent<Camera>();
        }
    }
}