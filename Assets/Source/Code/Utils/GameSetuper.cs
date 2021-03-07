using Photon.Pun;
using Photon.Realtime;
using Source.Code.Cam;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using Source.Code.PlayerInput;
using Source.Code.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class GameSetuper : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool isOffline = false;
#endif
        [Header("System links")]
        [SerializeField] private Transform systemsRoot;
        [SerializeField] private UnitSpawner unitSpawner;
        [SerializeField] private GameUIWindowsSwitcher uiSwitcher;
        [Header("Manual setup")]
        [SerializeField] private SessionSetup manualSetupSettings;

        private SessionSetup setupSettings;

        private void Awake()
        {
#if UNITY_EDITOR
            if (isOffline)
            {
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6 });
            }
#endif

            Debug.Log($"Offline mode is = {PhotonNetwork.OfflineMode}");
            if (PhotonNetwork.OfflineMode)
            {
                manualSetupSettings.ManualInitialize();
                setupSettings = manualSetupSettings;
            }
            else
            {
                Debug.Log("Found setup settings from last scene");
                setupSettings = LoadSetupSettings();
            }

            var sessionSettings = SessionSettings.New(systemsRoot, setupSettings);
            unitSpawner.Initialize(sessionSettings);

            int currentPlayerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            int currentPlayerID = 0;
            int currentPlayerFactionID = 0;

            foreach (var faction in setupSettings.Factions)
            {
                bool isFind = false;
                foreach (var player in faction.Value)
                {
                    if (currentPlayerActorNumber == player.OwnerActorNumber)
                    {
                        currentPlayerID = player.PlayerOrdinalID;
                        currentPlayerFactionID = player.FactionID;
                        isFind = true;
                        break;
                    }
                }
                if (isFind) break;
            }

            sessionSettings.SetCurrentPlayerData(currentPlayerID, currentPlayerFactionID);

            var inputSystem = PlayerInputSystem.New(systemsRoot, sessionSettings);

            var globalState = new GlobalState(unitSpawner, setupSettings, systemsRoot, inputSystem);
            var localState = new LocalState(globalState, inputSystem);
            sessionSettings.SetGameStates(globalState, localState);

            uiSwitcher.Initialize();

            var virtualCamera = VirtualCamera.New(systemsRoot, inputSystem.LookPivot.Transform);

            Faction.Initialize(sessionSettings, unitSpawner);

            var playerSettings = sessionSettings.SetupSettings.Players[currentPlayerActorNumber];
            unitSpawner.SpawnUnit(playerSettings, sessionSettings.Factions[playerSettings.FactionID].GetControlledUnitSpawnZone(playerSettings));

            //TEMP
            globalState.PreStartGame();
            //-----------------------------------
        }

        private SessionSetup LoadSetupSettings()
        {
            var setup = new SessionSetup();
            setup.AutoIntitialize();
            return setup;
        }
    }

    [Serializable]
    public struct PlayerSettings
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private int playerID;
        [SerializeField] private int ownerActorNumber;
        [SerializeField] private int factionID;

        private string unitPrefabName;

        public string UnitPrefabName 
        { 
            get
            {
                if (unitPrefabName == null)
                {
                    unitPrefabName = unitPrefab.name;
                }
                return unitPrefabName;
            }
            private set
            {
                unitPrefabName = value;
            }
        }

        public int PlayerOrdinalID => playerID;
        public int OwnerActorNumber => ownerActorNumber;
        public int FactionID => factionID;

        public PlayerSettings(int ownerActorNumber, int playerOrdinalID, int factionID, string unitPrefabName)
        {
            this.ownerActorNumber = ownerActorNumber;
            playerID = playerOrdinalID;
            this.factionID = factionID;
            this.unitPrefabName = unitPrefabName;
            unitPrefab = null;
        }
    }

    [Serializable]
    public class SessionSetup
    {
        [SerializeField] private PlayerSettings[] players;
        [Header("Game settings")]
        [SerializeField] private float respawnDurationInSec = 5f;
        [SerializeField] private int matchDurationInSec = 300;
        [SerializeField] private int scoresToWin = 50;
        [SerializeField] private int scoresFromKill = 1;

        public Dictionary<int, List<PlayerSettings>> Factions { get; private set; } = new Dictionary<int, List<PlayerSettings>>(); //factionID, players
        public Dictionary<int, PlayerSettings> Players { get; private set; } = new Dictionary<int, PlayerSettings>();//actorNR, playerSettings 
        public float RespawnDuration => respawnDurationInSec;
        public int MatchDuration => matchDurationInSec;
        public int ScoresToWin => scoresToWin;
        public int ScoresFromKill => scoresFromKill;

        public void AutoIntitialize()
        {
            var players = PhotonNetwork.PlayerList;

            for (int i = 0; i < players.Length; i++)
            {
                int factionID = PhotonExtensions.GetValueOrReturnDefault<int>(players[i].CustomProperties, GlobalConst.PLAYER_FACTION);
                string unitPrefabName = PhotonExtensions.GetValueOrReturnDefault<string>(players[i].CustomProperties, GlobalConst.PLAYER_UNIT_PREFAB_NAME);
                Debug.Log($"Player {players[i].NickName} in faction {factionID}, unit path = {unitPrefabName}");
                var newPlayerSettings = new PlayerSettings(players[i].ActorNumber, i, factionID, unitPrefabName);

                if (Factions.ContainsKey(factionID)) Factions[factionID].Add(newPlayerSettings);
                else Factions.Add(factionID, new List<PlayerSettings> { newPlayerSettings });

                Players.Add(players[i].ActorNumber, newPlayerSettings);
            }

            Debug.Log(PhotonNetwork.CurrentRoom);
            var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProperties.TryGetValue(GlobalConst.ROOM_MATCH_DURATION, out object matchDuration)) matchDurationInSec = Mathf.RoundToInt((float)matchDuration);
            if (roomProperties.TryGetValue(GlobalConst.ROOM_POINTS_TO_WIN, out object pointsToWin)) scoresToWin = Mathf.RoundToInt((float)pointsToWin);
            if (roomProperties.TryGetValue(GlobalConst.ROOM_UNIT_RESPAWN_DURATION, out object unitRespDuration)) respawnDurationInSec = (float)unitRespDuration;
        }

        public void ManualInitialize()
        {
            for (int i = 0; i < players.Length; i++)
            {
                int factionID = players[i].FactionID;

                if (Factions.ContainsKey(factionID)) Factions[factionID].Add(players[i]);
                else Factions.Add(factionID, new List<PlayerSettings> { players[i] });

                Players.Add(players[i].OwnerActorNumber, players[i]);
            }
        }
    }
}
