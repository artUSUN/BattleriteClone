using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Photon
{
    public class PlayerMover : MonoBehaviourPun
    {
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private float rotateSpeed = 10;
        [SerializeField] private float jumpHeight = 2;
        [SerializeField] private float gravityValue = -9.81f;

        private CharacterController cc;
        private Transform tr;
        private Vector3 playerVelocity;
        private bool groundedPlayer;

        #region MonoBehaviour Callbacks

        private void Start()
        {
            cc = GetComponentInChildren<CharacterController>();
            tr = transform;
        }

        private void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true) return;

            float hor = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");

            Rotate(hor);

            groundedPlayer = cc.isGrounded;

            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 direction = tr.forward * vert;
            cc.Move(direction * Time.deltaTime * moveSpeed);

            // Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            cc.Move(playerVelocity * Time.deltaTime);
        }

        #endregion

        private void Rotate(float horValue)
        {
            tr.Rotate(Vector3.up * horValue * rotateSpeed * Time.deltaTime);
        }
    }
}
