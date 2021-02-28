using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class StartMatchButton : MonoBehaviour
    {
        public enum ButtonStates
        {
            NotEnoughPlayers,
            WaitingForPlayers,
            AllPlayersReady,
            NotInitialzied
        }

        [SerializeField] private TextMeshProUGUI startMatchTMP;
        [SerializeField] private TextMeshProUGUI waitingTMP;
        [SerializeField] private TextMeshProUGUI countOfReadyTMP;
        [SerializeField] private TextMeshProUGUI notEnoughPlayersTMP;
        [SerializeField] private Button button;

        public ButtonStates State { get; private set; } = ButtonStates.NotInitialzied;

        public void SetCountOfWaitingPlayers(int readyPlayers, int totalPlayers)
        {
            countOfReadyTMP.text = $"{readyPlayers} / {totalPlayers} ready";
        }

        public void SetState(ButtonStates state)
        {
            if (State == state) return;

            button.interactable = false;
            DisableAllTMP();

            switch (state)
            {
                case ButtonStates.NotEnoughPlayers:
                    notEnoughPlayersTMP.gameObject.SetActive(true);
                    break;
                case ButtonStates.WaitingForPlayers:
                    waitingTMP.gameObject.SetActive(true);
                    countOfReadyTMP.gameObject.SetActive(true);
                    break;
                case ButtonStates.AllPlayersReady:
                    startMatchTMP.gameObject.SetActive(true);
                    button.interactable = true;
                    break;
            }
        }

        private void DisableAllTMP()
        {
            startMatchTMP.gameObject.SetActive(false);
            waitingTMP.gameObject.SetActive(false);
            countOfReadyTMP.gameObject.SetActive(false);
            notEnoughPlayersTMP.gameObject.SetActive(false);
        }
    }
}