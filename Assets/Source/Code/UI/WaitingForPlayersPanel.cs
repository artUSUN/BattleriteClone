using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using Source.Code.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Source.Code.UI
{
    public class WaitingForPlayersPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RectTransform placePlayersNamesTransform;
        [SerializeField] private GameObject playerNicknamePrefab;

        private Dictionary<int, GameObject> dontReadyPlayers = new Dictionary<int, GameObject>();

        public void Initialize()
        {
            gameObject.SetActive(true);

            var players = PhotonNetwork.PlayerList;
            foreach (var player in players)
            {
                bool isLoaded = PhotonExtensions.GetValueOrReturnDefault<bool>(player.CustomProperties, GlobalConst.PLAYER_LOADED_LEVEL);
                if (!isLoaded)
                {
                    var nickGO = Instantiate(playerNicknamePrefab, placePlayersNamesTransform);
                    nickGO.GetComponent<TextMeshProUGUI>().text = player.NickName;
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(GlobalConst.PLAYER_LOADED_LEVEL, out object result))
            {
                if ((bool)result)
                {
                    if (dontReadyPlayers.TryGetValue(targetPlayer.ActorNumber, out GameObject nickNameGO))
                    {
                        Destroy(nickNameGO);
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                bool isAllLoaded = true;
                var players = PhotonNetwork.PlayerList;
                foreach (var player in players)
                {
                    bool isLoaded = PhotonExtensions.GetValueOrReturnDefault<bool>(player.CustomProperties, GlobalConst.PLAYER_LOADED_LEVEL);
                    if (!isLoaded == false)
                    {
                        isAllLoaded = false;
                        break;
                    }
                }

                if (isAllLoaded)
                {
                    SessionSettings.Instance.GlobalState.SetPreGameTimer(0);
                }
            }
        }
    }
}