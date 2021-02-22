using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.MyPhoton
{
    public class ConnectionToRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI debugTMP;
        [SerializeField] private Button[] buttons;


        private void Awake()
        {
            foreach (var button in buttons) button.interactable = false;
            debugTMP.text = "";
        }

        public void OnCreateNewRoomButtonClick()
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6});
        }

        public void OnJoinToRandomRoomClick()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnConnectedToMaster()
        {
            foreach (var button in buttons) button.interactable = true;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            foreach (var button in buttons) button.interactable = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string debugText = $"OnJoinRandomFailed() was called by PUN. No random room available. Short: {returnCode} Message: {message}";
            Debug.LogError(debugText);
            debugTMP.text = debugText;
            debugTMP.color = Color.red;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            string debugText = $"OnCreateRoomFailed() was called by PUN. Short: {returnCode} Message: {message}";
            Debug.LogError(debugText);
            debugTMP.text = debugText;
            debugTMP.color = Color.red;
        }
    }
}