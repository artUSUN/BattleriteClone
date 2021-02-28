using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class StartTimer : MonoBehaviour
    {
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerTMP;

        private Coroutine timerCoroutine;
        private WaitForSeconds waitASec;

        public event Action TimerEnded;

        public void Play()
        {
            timerImage.enabled = true;
            timerTMP.enabled = true;
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }

        public void Stop()
        {
            timerImage.enabled = false;
            timerTMP.enabled = false;
            if (timerCoroutine == null) return;
            StopCoroutine(timerCoroutine);
        }

        private IEnumerator TimerCoroutine()
        {
            waitASec = new WaitForSeconds(1);

            for (int i = 3; i > 0; i--)
            {
                timerTMP.text = i.ToString();
                yield return waitASec;
            }
            timerTMP.text = "0";
            TimerEnded?.Invoke();
        }
    }
}