using Source.Code.Utils;
using TMPro;
using UnityEngine;

namespace Source.Code.UI
{
    public class MatchTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerTMP;

        private SessionSettings sessionSettings;
        private int lastTimerValue;
        private int matchDuration;

        private void Start()
        {
            sessionSettings = SessionSettings.Instance;
            matchDuration = sessionSettings.SetupSettings.MatchDuration;
        }

        private void Update()
        {
            int curValue = matchDuration - Mathf.FloorToInt(sessionSettings.GlobalState.MatchTime);
            if (curValue != lastTimerValue) SetTime(curValue);
            lastTimerValue = curValue;
        }

        private void SetTime(int time)
        {
            string formatedTime = $"{time/60, 0:d2}:{time%60, 0:d2}";
            timerTMP.text = formatedTime;
        }
    }
}