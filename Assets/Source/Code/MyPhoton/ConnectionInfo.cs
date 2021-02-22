using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.MyPhoton
{
    public class ConnectionInfo : MonoBehaviour
    {
        [SerializeField] private ConnectionToMaster master;

        [SerializeField] private TextMeshProUGUI pingTMP;
        [SerializeField] private TextMeshProUGUI countOfPlayers;
        [SerializeField] private TextMeshProUGUI roomCountTMP;
        [SerializeField] private Button refreshButton;

        private void Awake()
        {
            master.Disconnected += OnDisconnected;
            master.ConnectedToMaster += OnConnected;
            
            refreshButton.interactable = false;
        }

        private void OnDisable()
        {
            master.Disconnected += OnDisconnected;
            master.ConnectedToMaster += OnConnected;
        }
            
        public void RefreshButton()
        {
            ShowInfo();
        }

        private void OnConnected()
        {
            refreshButton.interactable = true;
            ShowInfo();
        }

        private void OnDisconnected()
        {
            refreshButton.interactable = false;
        }

        private void ShowInfo()
        {
            pingTMP.text = $"{PhotonNetwork.GetPing()}";
            countOfPlayers.text = PhotonNetwork.CountOfPlayers.ToString();
            roomCountTMP.text = PhotonNetwork.CountOfRooms.ToString();
        }
    }
}
