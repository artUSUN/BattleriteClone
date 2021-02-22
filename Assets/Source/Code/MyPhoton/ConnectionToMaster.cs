using Photon.Pun;
using Photon.Realtime;
using Source.Code.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Source.Code.MyPhoton
{
    public class ConnectionToMaster : MonoBehaviourPunCallbacks
    {
        [SerializeField] private NickNameSetter nickSetter;
        [SerializeField] private TextMeshProUGUI debugTMP;
        [SerializeField] private Transform afterConnectedCanvas;
        [SerializeField] private Button connectButton;

        public event Action ConnectedToMaster;
        public event Action Disconnected;

        private void Start()
        {
            debugTMP.text = "";
        }

        public void ConnectButton()
        {
            if (nickSetter.IsNicknameEmpty()) return;

            if (PhotonNetwork.IsConnected) return;

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = GlobalSettings.Version;
            debugTMP.text = "Connecting...";
            connectButton.interactable = false;
        }

        public void MainMenuButton()
        {
            Disconnected = null;
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }

        public override void OnConnectedToMaster()
        {
            string text = $"Successfully connected to PUN master server. The current region is \"{PhotonNetwork.CloudRegion}\"";
            Debug.Log(text);
            debugTMP.text = text;
            debugTMP.color = Color.green;

            afterConnectedCanvas.gameObject.SetActive(true);

            ConnectedToMaster?.Invoke();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            string text = $"OnDisconnected() was called by PUN with reason {cause}";
            Debug.LogError(text);
            debugTMP.text = text;
            debugTMP.color = Color.red;

            afterConnectedCanvas.gameObject.SetActive(false);
            connectButton.interactable = true;

            Disconnected?.Invoke();
        }
    }
}