using Source.Code.Cam;
using Source.Code.PlayerInput;
using System;
using UnityEngine;

namespace Source.Code.Utils
{
    public class GameSetuper : MonoBehaviour
    {
        [Header("System links")]
        [SerializeField] private Transform systemsRoot;
        [SerializeField] private UnitSpawner unitSpawner;
        [Header("Manual setup")]
        [SerializeField] private SessionSetup manualSetupSettings;

        private SessionSetup setupSettings;

        private bool setupDone = false;

        private void Awake()
        {
            if (setupSettings == null) setupSettings = manualSetupSettings;
            else Debug.Log("Found setup settings from last scene");

            var sessionSettings = SessionSettings.New(systemsRoot);

            //TEMP
            sessionSettings.InitPlayerID(0);
            //-----------------------------------

            unitSpawner.InitSpawn(setupSettings);

            var inputSystem = PlayerInputSystem.New(systemsRoot, sessionSettings);

            var virtualCamera = VirtualCamera.New(systemsRoot, inputSystem.LookPivot.Transform);

            var gameStates = new GameState(unitSpawner, inputSystem, virtualCamera, setupSettings);

            //TEMP
            gameStates.StartGame();
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
        public int PlayerID => playerID;
        public int FactionID => factionID;
    }

    [Serializable]
    public class SessionSetup
    {
        [SerializeField] private PlayerSettings[] players;

        public PlayerSettings[] Players => players;
    }
}
