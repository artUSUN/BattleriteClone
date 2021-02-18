using Source.Code.Cam;
using Source.Code.PlayerInput;
using System;

namespace Source.Code.Utils
{
    public class GameState
    {
        public enum GlobalStates
        {
            PreGame,
            Game,
            GameEnded
        }

        private UnitSpawner unitSpawner;
        private PlayerInputSystem inputSystem;
        private VirtualCamera virtualCamera;
        private SessionSettings sessionSettings;
        private SessionSetup setupSettings;

        public LocalState LocalState { get; private set; }
        public GlobalStates CurrentGlobalState { get; private set; } = GlobalStates.PreGame;

        public event Action<GlobalStates> GlobalStateChanged;

        public GameState(UnitSpawner unitSpawner, PlayerInputSystem inputSystem, VirtualCamera virtualCamera, SessionSetup setupSettings)
        {
            this.unitSpawner = unitSpawner;
            this.inputSystem = inputSystem;
            this.virtualCamera = virtualCamera;
            this.setupSettings = setupSettings;
            sessionSettings = SessionSettings.Instance;
            LocalState = new LocalState(this);
        }

        public void StartGame()
        {
            var factions = unitSpawner.InitSpawn(setupSettings);

            SessionSettings.Instance.InitFactions(factions);

            //TODO: сделать инициализацию отсюда и подвязывать юнита к ивентам

            CurrentGlobalState = GlobalStates.Game;
            GlobalStateChanged?.Invoke(CurrentGlobalState);
        }

    }

    public class LocalState
    {
        public enum LocalStates
        {
            Player,
            Spectator
        }

        public LocalStates CurrentLocalState { get; private set; } = LocalStates.Spectator;


        public LocalState(GameState gameState)
        {
            gameState.GlobalStateChanged += OnGlobalStateChanged;
        }

        private void OnGlobalStateChanged(GameState.GlobalStates currentGlobalState)
        {
            switch (currentGlobalState)
            {
                case GameState.GlobalStates.Game:
                    {
                        CurrentLocalState = LocalStates.Player;
                    }
                    break;
                case GameState.GlobalStates.GameEnded:
                    break;
                default:
                    break;
            }
        }
    }

}