using Photon.Pun;
using UnityEngine;

namespace Source.Code.Photon
{
    public class PlayerCameraSetter : MonoBehaviourPun
    {
        [SerializeField] private Vector3 camLocalPos;
        private Transform cam;

        private void Start()
        {
            cam = Camera.main.transform;

            if (photonView.IsMine)
            {
                cam.SetParent(transform);
                cam.localPosition = camLocalPos;
            } 
        }
    }
}
