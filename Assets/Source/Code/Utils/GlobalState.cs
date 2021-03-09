using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.MyPhoton;
using Source.Code.PlayerInput;
using Source.Code.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class GlobalState
    {
        public enum States
        {
            Default,
            WaitingForOtherPlayers,
            PreGameTimer,
            Game,
            GameEnded
        }

        private SessionSettings sessionSettings;
        private SessionSetup setupSettings;
        private PlayerInputSystem inputSystem;
        private Transform systemsRoot;
        private Timer matchTimer;
        private Timer preMatchTimer;

        public List<Faction> FactionsSortedByScore = new List<Faction>();
        public float MatchTime => matchTimer.Time;
        public float PreMatchTime => preMatchTimer.Time;
        public States Current { get; private set; } = States.Default;


        public event Action<States> GlobalStateChanged;

        public GlobalState(SessionSetup setupSettings, Transform systems, PlayerInputSystem inputSystem)
        {
            this.setupSettings = setupSettings;
            this.inputSystem = inputSystem;
            systemsRoot = systems;
            sessionSettings = SessionSettings.Instance;
            matchTimer = Timer.New(setupSettings.MatchDuration, systemsRoot);
        }

        public void SetWaitingForPlayersState()
        {
            if (Current != States.Default)
            {
                Debug.LogError($"Try switch to Pre Game State when in {Current} State");
                return;
            }
            Current = States.WaitingForOtherPlayers;
            Debug.Log($"Current state is {Current}");

            GlobalStateChanged?.Invoke(Current);

            if (PhotonNetwork.OfflineMode == true)
            {
                SetPreGameTimer(0);
            }
        }

        public void SetPreGameTimer(float lag)
        {
            if (Current != States.WaitingForOtherPlayers)
            {
                Debug.LogError($"Try switch to Pre Game State when in {Current} State");
                return;
            }
            Current = States.PreGameTimer;
            Debug.Log($"Current state is {Current}");

            float timerDuration = 3 - lag;
            preMatchTimer = Timer.New(timerDuration, systemsRoot);
            preMatchTimer.Play();
            preMatchTimer.Ended += StartGame;

            object content = (float)PhotonNetwork.Time;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(GlobalConst.ROOM_START_MATCH_BEGIN, content, raiseEventOptions, SendOptions.SendReliable);

            GlobalStateChanged?.Invoke(Current);
        }

        public void StartGame()
        {
            if (Current != States.PreGameTimer)
            {
                Debug.LogError($"Try switch to Game State when in {Current} State");
                return;
            }

            Current = States.Game;
            Debug.Log($"Current state is {Current}");

            preMatchTimer.Ended -= StartGame;

            foreach (var faction in sessionSettings.Factions) faction.ScoresChanged += CheckScores;

            matchTimer.Ended += EndGame;
            matchTimer.Play();

            GlobalStateChanged?.Invoke(Current);
        }

        public void EndGame()
        {
            if (Current != States.Game)
            {
                Debug.LogError($"Try switch to EndGame State when in {Current} State");
                return;
            }

            matchTimer.Ended -= EndGame;

            Current = States.GameEnded;
            Debug.Log($"Current state is {Current}");

            FactionsSortedByScore.AddRange(sessionSettings.Factions);
            FactionsSortedByScore.Sort();
            FactionsSortedByScore.Reverse();

            inputSystem.gameObject.SetActive(false);
            matchTimer.Pause();

            GlobalStateChanged?.Invoke(Current);
        }

        private void CheckScores(int score)
        {
            if (score >= setupSettings.ScoresToWin) EndGame();
        }
    }

    public class Timer : MonoBehaviour
    {
        public float Time { get; private set; } = 0;
        public float TimeBeforeEnd { get; private set; }
        public float Duration { get; private set; }

        public event Action Ended;

        public static Timer New(float duration, Transform parent)
        {
            var newGO = new GameObject("Timer");
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

        public void Pause()
        {
            StopAllCoroutines();
        }

        public void Stop()
        {
            StopAllCoroutines();
            Time = 0;
            TimeBeforeEnd = Duration;
        }

        private IEnumerator TimerCoroutine()
        {
            TimeBeforeEnd = Duration - Time;

            while (Time < Duration)
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

        private void OnGlobalStateChanged(GlobalState.States currentGlobalState)
        {
            switch (currentGlobalState)
            {
                case GlobalState.States.Game:
                    {
                        SetLocalState(LocalStates.Player);
                    }
                    break;
                case GlobalState.States.GameEnded:
                    {

                    }
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

        private void OnControlledUnitDied(Unit who)
        {
            Debug.Log("OnControlledUnitDied");
            SetLocalState(LocalStates.Spectator);

        }

        private void OnControlledUnitSet(Unit unit)
        {
            var controlledUnitUnderline = GlobalSettingsLoader.Load().Prefabs.ControlledUnitUnderline;
            MonoBehaviour.Instantiate(controlledUnitUnderline, sessionSettings.ControlledUnit.Model);
        }
    }

}