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
        [Header("System links")]
        [SerializeField] private Transform systemsRoot;
        [SerializeField] private UnitSpawner unitSpawner;
        [SerializeField] private GameUIWindowsSwitcher uiSwitcher;
        [Header("Manual setup")]
        [SerializeField] private SessionSetup manualSetupSettings;

        private SessionSetup setupSettings;

        private void Awake()
        {
            PhotonNetwork.OfflineMode = true;

            if (PhotonNetwork.OfflineMode) setupSettings = manualSetupSettings;
            else
            {
                Debug.Log("Found setup settings from last scene");
                setupSettings = LoadSetupSettings();
            }

            var sessionSettings = SessionSettings.New(systemsRoot, setupSettings);

            //TEMP
            sessionSettings.SetPlayerID(0);
            //-----------------------------------

            var inputSystem = PlayerInputSystem.New(systemsRoot, sessionSettings);

            var globalState = new GlobalState(unitSpawner, setupSettings, systemsRoot, inputSystem);
            var localState = new LocalState(globalState, inputSystem);
            sessionSettings.SetGameStates(globalState, localState);

            uiSwitcher.Initialize();

            var virtualCamera = VirtualCamera.New(systemsRoot, inputSystem.LookPivot.Transform);
            //TEMP
            globalState.PreStartGame();
            //-----------------------------------
        }

        private SessionSetup LoadSetupSettings()
        {
            return null;
            //return new SessionSetup();
        }
    }

    [Serializable]
    public struct PlayerSettings
    {
        [SerializeField] private GameObject unitPrefab;
        //[SerializeField] private string unitPrefabName;
        [SerializeField] private int playerID;
        [SerializeField] private int ownerActorNumber;
        [SerializeField] private int factionID;

        public GameObject UnitPrefab => unitPrefab;
        //public string UnitPrefab => unitPrefabName;
        public int PlayerOrdinalID => playerID;
        public int OwnerActorNumber => ownerActorNumber;
        public int FactionID => factionID;

        //public PlayerSettings(int ownerActorNumber, int playerOrdinalID, int factionID, string unitPrefab)
        //{
        //    this.ownerActorNumber = ownerActorNumber;
        //    playerID = playerOrdinalID;
        //    this.factionID = factionID;
        //    this.unitPrefabName = unitPrefab;
        //}
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

        public PlayerSettings[] Players => players;
        public float RespawnDuration => respawnDurationInSec;
        public int MatchDuration => matchDurationInSec;
        public int ScoresToWin => scoresToWin;
        public int ScoresFromKill => scoresFromKill;

        //public SessionSetup()
        //{
        //    var players = PhotonNetwork.PlayerList;

        //    this.players = new PlayerSettings[players.Length];

        //    for (int i = 0; i < players.Length; i++)
        //    {
        //        int factionID = PhotonExtensions.GetValueOrReturnDefault<int>(players[i].CustomProperties, GlobalConst.PLAYER_FACTION); 
        //        string unitPrefabName = PhotonExtensions.GetValueOrReturnDefault<string>(players[i].CustomProperties, GlobalConst.PLAYER_UNIT_PREFAB_NAME);
        //        this.players[i] = new PlayerSettings(players[i].ActorNumber, i, factionID, unitPrefabName);
        //    }

        //    var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        //    if (roomProperties.TryGetValue(GlobalConst.ROOM_MATCH_DURATION, out object matchDuration)) matchDurationInSec = Mathf.RoundToInt((float)matchDuration);
        //    if (roomProperties.TryGetValue(GlobalConst.ROOM_POINTS_TO_WIN, out object pointsToWin)) scoresToWin = Mathf.RoundToInt((float)pointsToWin);
        //    if (roomProperties.TryGetValue(GlobalConst.ROOM_UNIT_RESPAWN_DURATION, out object unitRespDuration)) respawnDurationInSec = (float)unitRespDuration;
        //}
    }
}
