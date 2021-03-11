using ExitGames.Client.Photon;
using Photon.Pun;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class PhotonObserver : MonoBehaviourPunCallbacks
    {
        private Dictionary<Faction, string> factionScoresKeys = new Dictionary<Faction, string>();
        private SessionSettings sessionSettings;

        public static PhotonObserver New(SessionSettings sessionSettings, Transform root)
        {
            var newGO = new GameObject("PhotonObserver");
            newGO.transform.SetParent(root);
            var newObserver = newGO.AddComponent<PhotonObserver>();
            newObserver.Initialize(sessionSettings);
            return newObserver;
        }

        public void Initialize(SessionSettings sessionSettings)
        {
            this.sessionSettings = sessionSettings;
            for (int i = 0; i < sessionSettings.Factions.Length; i++)
            {
                factionScoresKeys.Add(sessionSettings.Factions[i], GlobalConst.GetFactionScoresKey(i));
                Debug.Log(GlobalConst.GetFactionScoresKey(i));
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
            Debug.Log("OnPropertiesUpdate");
            foreach (var faction in factionScoresKeys)
            {
                Debug.Log($"For faction {faction.Key.ID} with keyWord {faction.Value}");
                if (changedProps.TryGetValue(faction.Value, out object score))
                {
                    faction.Key.SetScore((int)score);
                    Debug.Log($"For faction {faction.Key.ID} find {(int)score}");
                }
            }
        }

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == GlobalConst.PRE_GAME_TIMER_STARTED)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    object[] data = (object[])photonEvent.CustomData;
                    float sendTime = (float)data[0];
                    sessionSettings.GlobalState.SetPreGameTimer((float)PhotonNetwork.Time - sendTime);
                }
            }
        }
    }
}