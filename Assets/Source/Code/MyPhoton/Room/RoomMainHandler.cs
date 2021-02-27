using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.UI.Room;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Code.MyPhoton.Room
{
    public class RoomMainHandler : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RoomCardSystem cardSystem;
        [SerializeField] private RoomUIHandler uiHandler;

        private Player[] players;

        private void Awake()
        {
            players = PhotonNetwork.PlayerList;

            bool isMaster = PhotonNetwork.IsMasterClient;

            uiHandler.Initialize(this, isMaster);

            if (isMaster)
            {
                RoomCardSystem.SetAvailableCardPlace(PhotonNetwork.LocalPlayer);
                MasterClientActions();
            }

            cardSystem.Initialize(players);
            

            //players[0].GetPlayerNumber

            //if (PhotonNetwork.IsMasterClient)
            //{
            //    cardSystem.InitializeOnMaster();
            //    MasterClientActions();
            //}
            //else
            //{
            //    cardSystem.Initialize(players);
            //}



        }

        private void Start()
        {
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_LOADED_LEVEL, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void OnPlayButton()
        {
            
        }


        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MasterClientActions();
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
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            cardSystem.OnPlayerLeaved(otherPlayer.ActorNumber);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object newPosition))
            {
                cardSystem.OnCardPositionChanged(targetPlayer, (int)newPosition);
            }

            if (changedProps.TryGetValue(GlobalConst.PLAYER_LOADED_LEVEL, out object isLoaded))
            {
                if ((bool)isLoaded)
                {
                    cardSystem.OnPlayerLoadedGame(targetPlayer);
                }
            }

            if (changedProps.TryGetValue(GlobalConst.PLAYER_READY, out object isReady))
            {
                cardSystem.SetPlayerReady(targetPlayer, (bool)isReady);
            }
        }

        public override void OnLeftRoom()
        {
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_READY, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            SceneManager.LoadScene(1);
        }

        public void SetPlayerReady(bool isReady)
        {
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_READY, isReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        private void MasterClientActions()
        {
            //set cards and settings fields interactive

            Hashtable props = new Hashtable { { GlobalConst.PLAYER_READY, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}