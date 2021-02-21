using Source.Code.Cam;
using Source.Code.PlayerInput;
using Source.Code.Units;
using System;
using System.Collections;
using UnityEngine;

namespace Source.Code.Utils
{
    public class GlobalState
    {
        public enum GlobalStates
        {
            PreGame,
            Game,
            GameEnded
        }

        private Transform cameraBorders;

        private UnitSpawner unitSpawner;
        private SessionSettings sessionSettings;
        private SessionSetup setupSettings;
        private Transform systemsRoot;
        private Timer matchTimer;

        public float MatchTime => matchTimer.Time;
        public GlobalStates Current { get; private set; } = GlobalStates.PreGame;

        public event Action<GlobalStates> GlobalStateChanged;

        public GlobalState(UnitSpawner unitSpawner, SessionSetup setupSettings, Transform systems)
        {
            this.unitSpawner = unitSpawner;
            this.setupSettings = setupSettings;
            systemsRoot = systems;
            sessionSettings = SessionSettings.Instance;
            matchTimer = Timer.New(setupSettings.MatchDuration, systemsRoot);
        }

        public void StartGame()
        {
            unitSpawner.InitSpawn(setupSettings);

            matchTimer.Ended += EndGame;
            matchTimer.Play();

            Current = GlobalStates.Game;
            GlobalStateChanged?.Invoke(Current);
        }

        public void EndGame()
        {
            matchTimer.Ended -= EndGame;


        }
    }

    public class Timer : MonoBehaviour
    {
        public float Time { get; private set; }
        public float TimeBeforeEnd { get; private set; }
        public float Duration { get; private set; }

        public event Action Ended;

        public static Timer New(float duration, Transform parent)
        {
            var newGO = new GameObject();
            newGO.transform.SetParent(parent);
            var timer = newGO.AddComponent<Timer>();
            timer.Duration = duration;
            return timer;
        }

        public void Play()
        {
            if (TimeBeforeEnd != 0)
            {
                Debug.LogError("The timer has already started", transform);
                return;
            }

            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            Time = 0;
            TimeBeforeEnd = Duration;

            while (Time >= Duration)
            {
                Time += UnityEngine.Time.deltaTime;
                TimeBeforeEnd -= UnityEngine.Time.deltaTime;
                yield return null; 
            }

            TimeBeforeEnd = 0;

            Ended?.Invoke();
        }
    }


    public class LocalState
    {
        public enum LocalStates
        {
            Player,
            Spectator
        }

        private SessionSettings sessionSettings;
        private PlayerInputSystem inputSystem;

        public LocalStates Current { get; private set; } = LocalStates.Spectator;
        public event Action<LocalStates> StateChanged;

        public LocalState(GlobalState globalState, PlayerInputSystem inputSystem)
        {
            sessionSettings = SessionSettings.Instance;
            globalState.GlobalStateChanged += OnGlobalStateChanged;
            sessionSettings.ControlledUnitSet += OnControlledUnitSet;

            this.inputSystem = inputSystem;
        }

        private void OnGlobalStateChanged(GlobalState.GlobalStates currentGlobalState)
        {
            switch (currentGlobalState)
            {
                case GlobalState.GlobalStates.Game:
                    {
                        SetLocalState(LocalStates.Player);
                    }
                    break;
                case GlobalState.GlobalStates.GameEnded:
                    break;
                default:
                    break;
            }
        }

        private void SetLocalState(LocalStates state)
        {
            ExitState();
            Current = state;
            EnterState();
            StateChanged?.Invoke(Current);
        }

        private void ExitState()
        {
            switch (Current)
            {
                case LocalStates.Player:
                    {
                        inputSystem.UnsubscribeEvents();
                    }
                    break;
                case LocalStates.Spectator:
                    break;
            }
        }

        private void EnterState()
        {
            switch (Current)
            {
                case LocalStates.Player:
                    {
                        sessionSettings.ControlledUnit.SubscribeToEvents(inputSystem);
                        sessionSettings.ControlledUnit.HealthComponent.Died += OnControlledUnitDied;
                    }
                    break;
                case LocalStates.Spectator:
                    break;
            }
        }

        private void OnControlledUnitDied(Unit who, Unit from)
        {
            SetLocalState(LocalStates.Spectator);

        }

        private void OnControlledUnitSet(Unit unit)
        {
            if (sessionSettings.GlobalState.Current == GlobalState.GlobalStates.Game)
            {
                SetLocalState(LocalStates.Player);
            }
        }
    }

}