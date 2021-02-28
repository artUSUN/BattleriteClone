﻿using Photon.Realtime;
using Source.Code.Cam;
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

        private bool setupDone = false;

        private void Awake()
        {
            if (setupSettings == null) setupSettings = manualSetupSettings;
            else Debug.Log("Found setup settings from last scene");

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

        public void SetSettings(SessionSetup settings)
        {
            if (setupDone)
            {
                Debug.LogError("The settings are already set", transform);
                return;
            }

            setupDone = true;

            setupSettings = settings;
        }
    }

    [Serializable]
    public struct PlayerSettings
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private int playerID;
        [SerializeField] private int factionID;

        public GameObject UnitPrefab => unitPrefab;
        public int PlayerOrdinalID => playerID;
        public Player Owner { get; private set; }
        public int FactionID => factionID;

        public PlayerSettings(Player owner, int playerOrdinalID, int factionID, GameObject unitPrefab)
        {
            Owner = owner;
            playerID = playerOrdinalID;
            this.factionID = factionID;
            this.unitPrefab = unitPrefab;
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

        public PlayerSettings[] Players => players;
        public float RespawnDuration => respawnDurationInSec;
        public int MatchDuration => matchDurationInSec;
        public int ScoresToWin => scoresToWin;
        public int ScoresFromKill => scoresFromKill;

        public SessionSetup(PlayerSettings[] players, Dictionary<string, float> fieldsSettings)
        {

        }
    }
}
