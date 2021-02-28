using Photon.Pun;
using Source.Code.MyPhoton;
using Source.Code.MyPhoton.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class RoomUIHandler : MonoBehaviour
    {
        [Header("Other UI handlers")]
        [SerializeField] private SettingsUI settingsUI;
        [SerializeField] private StartTimer timer;
        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        [SerializeField] private StartMatchButton startButton;

        private bool isPlayerReady = false;
        private RoomMainHandler roomMainHandler;

        public StartTimer StartTimer => timer;


        public void Initialize(RoomMainHandler roomMainHandler, bool isMaster)
        {
            this.roomMainHandler = roomMainHandler;

            CheckPlayersCount();

            if (isMaster) EnableStartButton();
            else EnableReadyButton();

            settingsUI.Initialize(isMaster);
        }

        public void OnPlayerEntered()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckPlayersCount();
            }
        }

        public void OnPlayerReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckPlayersCount();
            }
        }

        public void OnStayMasterClient()
        {
            EnableStartButton();
            CheckPlayersCount();
            settingsUI.SetFieldsInteractable(true);
        }

        public void OnPlayerLeave()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckPlayersCount();
                settingsUI.SetFieldsInteractable(true);
            }
        }

        public void OnLeaveLobbyButton()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnStartButton()
        {
            if (CheckPlayersCount())
            {
                settingsUI.SetFieldsInteractable(false);
                roomMainHandler.OnMatchButtonClicked();
            }
        }

        public void OnReadyButton()
        {
            isPlayerReady = !isPlayerReady;
            roomMainHandler.SetPlayerReady(isPlayerReady);
            SetReadyButtonState(isPlayerReady);
        }

        private void EnableStartButton()
        {
            startButton.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(false);
        }

        private void EnableReadyButton()
        {
            startButton.gameObject.SetActive(false);
            readyButton.gameObject.SetActive(true);
        }

        private void SetReadyButtonState(bool isReady)
        {
            readyButton.GetComponent<Image>().color = isReady ? Color.green : Color.red;
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = isReady ? "Ready!" : "Ready?";
        }

        private bool CheckPlayersCount()
        {
            if (startButton.gameObject.activeSelf == false) return false;

            var players = PhotonNetwork.PlayerList;

            if (players.Length == 1)
            {
                startButton.SetState(StartMatchButton.ButtonStates.NotEnoughPlayers);
                return false;
            }

            int countOfReadyPlayers = 0;

            foreach (var player in players)
            {
                if (player.CustomProperties.TryGetValue(GlobalConst.PLAYER_READY, out object isReady))
                {
                    if ((bool)isReady) countOfReadyPlayers++;
                }
            }

            if (countOfReadyPlayers == players.Length)
            {
                startButton.SetState(StartMatchButton.ButtonStates.AllPlayersReady);
                return true;
            }
            else
            {
                startButton.SetState(StartMatchButton.ButtonStates.WaitingForPlayers);
                startButton.SetCountOfWaitingPlayers(countOfReadyPlayers, players.Length);
                return false;
            }
        }
    }
}
