using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.Extensions;
using Source.Code.UI.Room;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Code.MyPhoton.Room
{
    public class RoomMainHandler : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RoomCardSystem cardSystem;
        [SerializeField] private RoomUIHandler uiHandler;
        [SerializeField] private CardDragger cardDragger;
        [Header("Plug")]
        [SerializeField] private GameObject plugUnitPrefab;

        private Player[] players;

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            players = PhotonNetwork.PlayerList;

            bool isMaster = PhotonNetwork.IsMasterClient;

            uiHandler.Initialize(this, isMaster);

            if (isMaster)
            {
                RoomCardSystem.SetAvailableCardPlace(PhotonNetwork.LocalPlayer);
                MasterClientActions();
            }
            else
            {
                cardDragger.enabled = false;
            }

            uiHandler.StartTimer.TimerEnded += OnStartTimerEnded;

            cardSystem.Initialize(players);
        }



        private void Start()
        {
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_LOADED_LEVEL, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
            uiHandler.StartTimer.TimerEnded -= OnStartTimerEnded;
        }

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == GlobalConst.ROOM_START_MATCH_BEGIN)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    cardDragger.enabled = false;

                }
                uiHandler.StartTimer.Play();
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MasterClientActions();
                uiHandler.OnStayMasterClient();
            }
            cardSystem.OnMasterClientChanged(newMasterClient);
        }

        public override void OnPlayerEnteredRoom(Player enteredPlayer)
        {
            Debug.Log("Entered player. His ActorId = " + enteredPlayer.ActorNumber + " Name is " + enteredPlayer.NickName);
            if (PhotonNetwork.IsMasterClient)
            {
                RoomCardSystem.SetAvailableCardPlace(enteredPlayer);
            }
            uiHandler.OnPlayerEntered();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.IsOpen == false)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                cardDragger.enabled = true;
            }
            uiHandler.StartTimer.Stop();
            cardSystem.OnPlayerLeave(otherPlayer.ActorNumber);
            uiHandler.OnPlayerLeave();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object newPosition))
            {
                int newPosID = (int)newPosition;
                cardSystem.OnCardPositionChanged(targetPlayer, newPosID);
            }

            CheckPlayerLoaded(targetPlayer, changedProps);

            if (changedProps.TryGetValue(GlobalConst.PLAYER_READY, out object isReadyProperty))
            {
                bool isReady = (bool)isReadyProperty;
                uiHandler.OnPlayerReady();
                cardSystem.SetPlayerReady(targetPlayer, isReady);
            }
        }

        public void OnMatchButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            var players = PhotonNetwork.PlayerList;

            foreach (var player in players)
            {
                int factionID = cardSystem.GetPlayerFaction(player);

                Hashtable props = new Hashtable { { GlobalConst.PLAYER_FACTION, factionID } };
                player.SetCustomProperties(props);

                props = new Hashtable { { GlobalConst.PLAYER_UNIT_PREFAB_NAME, plugUnitPrefab.name } };
                player.SetCustomProperties(props);

                props = new Hashtable { { GlobalConst.PLAYER_LOADED_LEVEL, false } };
                player.SetCustomProperties(props);
            }

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(GlobalConst.ROOM_START_MATCH_BEGIN, null, raiseEventOptions, SendOptions.SendReliable);
        }


        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }

        public void SetPlayerReady(bool isReady)
        {
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_READY, isReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void OnStartTimerEnded()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("LoadLevel 3");
                PhotonNetwork.LoadLevel(3);
            }
        }

        private void CheckPlayerLoaded(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(GlobalConst.PLAYER_LOADED_LEVEL, out object isLoaded))
            {
                if ((bool)isLoaded)
                {
                    bool isReady = PhotonExtensions.GetValueOrReturnDefault<bool>(targetPlayer.CustomProperties, GlobalConst.PLAYER_READY);

                    cardSystem.SetPlayerReady(targetPlayer, isReady);
                }
            }
        }

        private void MasterClientActions()
        {
            //set cards and settings fields interactive
            cardDragger.enabled = true;

            Hashtable props = new Hashtable { { GlobalConst.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}