using Source.Code.Units.Components;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.Units.Bars
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private float backSliderWaitDuration = 1;
        [SerializeField] private float backSliderReduceDuration = 0.5f;
        [SerializeField] private Slider frontSlider;
        [SerializeField] private Slider backSlider;

        private WaitForSeconds backSliderWaiter;
        private HealthComponent health;

        public void Initialize(HealthComponent healthComponent)
        {
            health = healthComponent;
            health.HealthReduced += OnHealthReduced;
            health.HealthIncreased += OnHealthIncreased;
            backSliderWaiter = new WaitForSeconds(backSliderWaitDuration);
        }

        private void OnHealthReduced(float currentHP)
        {
            frontSlider.value = health.CurrentHPInPercent;
            StopAllCoroutines();
            StartCoroutine(ReduceBackSlider());
        }

        private void OnHealthIncreased(float currentHP)
        {
            frontSlider.value = health.CurrentHPInPercent;
            if (frontSlider.value >= backSlider.value)
            {
                StopAllCoroutines();
                backSlider.value = frontSlider.value;
            }
        }

        private IEnumerator ReduceBackSlider()
        {
            yield return backSliderWaiter;

            float backSliderValue = backSlider.value;
            float timer = 0;

            while (timer < backSliderReduceDuration)
            {
                backSlider.value = Mathf.Lerp(backSliderValue, frontSlider.value, timer / backSliderReduceDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            backSlider.value = frontSlider.value;
        }

    }
}