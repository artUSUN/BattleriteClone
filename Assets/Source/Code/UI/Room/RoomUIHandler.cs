using Photon.Pun;
using Source.Code.MyPhoton.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class RoomUIHandler : MonoBehaviour
    {
        private RoomMainHandler roomMainHandler;

        [SerializeField] private Button playButton;
        [SerializeField] private Button readyButton;

        private bool isPlayerReady = false;

        public void Initialize(RoomMainHandler roomMainHandler, bool isMaster)
        {
            this.roomMainHandler = roomMainHandler;

            if (isMaster)
            {
                readyButton.gameObject.SetActive(false);
                playButton.gameObject.SetActive(true);
                //ui elements interactive = true
            }
            else
            {
                readyButton.gameObject.SetActive(true);
                playButton.gameObject.SetActive(false);
                SetReadyButton(false);
            }
        }

        public void OnLeaveLobbyButton()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnPlayButton()
        {

        }

        public void OnReadyButton()
        {
            isPlayerReady = !isPlayerReady;
            roomMainHandler.SetPlayerReady(isPlayerReady);
            SetReadyButton(isPlayerReady);
        }

        private void SetReadyButton(bool isReady)
        {
            readyButton.GetComponent<Image>().color = isReady ? Color.green : Color.red;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = isReady ? "Ready" : "Ready?";
        }
    }
}
