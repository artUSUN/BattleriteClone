using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Code.Utils
{
    public class PhotonObserver : MonoBehaviourPunCallbacks
    {
        private Dictionary<Faction, string> factionScoresKeys = new Dictionary<Faction, string>();
        private SessionSettings sessionSettings;

        public void Initialize(SessionSettings sessionSettings)
        {
            this.sessionSettings = sessionSettings;
            for (int i = 0; i < sessionSettings.Factions.Length; i++)
            {
                factionScoresKeys.Add(sessionSettings.Factions[i], GlobalConst.GetFactionScoresKey(i));
            }
            
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
        }

        public override void OnRoomPropertiesUpdate(Hashtable changedProps)
        {
            foreach (var faction in factionScoresKeys)
            {
                int score = PhotonExtensions.GetValueOrReturnDefault<int>(changedProps, faction.Value);
                faction.Key.SetScore(score);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"Disconnected. DisconnectCause: {cause}");
            SceneManager.LoadScene(0);
        }

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == GlobalConst.GAME_PRE_GAME_TIMER_STARTED)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    float sendTime = (float)photonEvent.CustomData;
                    sessionSettings.GlobalState.SetPreGameTimer((float)PhotonNetwork.Time - sendTime);
                }
            }
        }
    }
}