using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.UI
{
    public class GameUIWindowsSwitcher : MonoBehaviour
    {
        [SerializeField] private TeamVsTeamScore scoreWindow;
        [SerializeField] private MatchTimer matchTimerWindow;
        [SerializeField] private EscMenu escMenu;
        [SerializeField] private WaitingForPlayersPanel waitingForPlayersPanel;

        private SessionSettings sessionSettings;

        public void Initialize()
        {
            sessionSettings = SessionSettings.Instance;
            sessionSettings.GlobalState.GlobalStateChanged += OnGlobalStateChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (escMenu.gameObject.activeSelf) escMenu.gameObject.SetActive(false);
                else escMenu.gameObject.SetActive(true);
            }
        }

        private void OnGlobalStateChanged(GlobalState.States globalState)
        {
            switch (globalState)
            {
                case GlobalState.States.WaitingForOtherPlayers:
                    {
                        waitingForPlayersPanel.Initialize();
                    }
                    break;
                case GlobalState.States.PreGameTimer:
                    {
                        waitingForPlayersPanel.gameObject.SetActive(false);
                    }
                    break;
                case GlobalState.States.Game:
                    {
                        matchTimerWindow.gameObject.SetActive(true);
                        scoreWindow.gameObject.SetActive(true);
                    }
                    break;
                case GlobalState.States.GameEnded:
                    break;
            }
        }
    }
}