using Source.Code.Units.Components;
using Source.Code.Utils;
using System.Collections;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI nickNameTMP;

        private WaitForSeconds backSliderWaiter;
        private HealthComponent health;

        public void Initialize(HealthComponent healthComponent, Unit unit)
        {
            health = healthComponent;
            health.HealthReduced += OnHealthReduced;
            health.HealthIncreased += OnHealthIncreased;
            backSliderWaiter = new WaitForSeconds(backSliderWaitDuration);
            SetNickName(unit);
        }

        private void SetNickName(Unit unit)
        {
            if (unit.OwnerNickName.Length != 0) nickNameTMP.text = unit.OwnerNickName;
            else nickNameTMP.text = $"Unit {unit.OwnerPlayerID}";

            SessionSettings sessionSettings = SessionSettings.Instance;

            bool isAlliasForControlledUnit = sessionSettings.CurrentPlayerFactionID == unit.Faction.ID;

            nickNameTMP.color = isAlliasForControlledUnit ? GlobalSettingsLoader.Load().Colors.AlliasColor : GlobalSettingsLoader.Load().Colors.EnemyColor;

            Color nickNameColor;

            if (isAlliasForControlledUnit)
            {
                if (sessionSettings.CurrentPlayerID == unit.OwnerPlayerID) nickNameColor = GlobalSettingsLoader.Load().Colors.OwnerColor;
                else nickNameColor = GlobalSettingsLoader.Load().Colors.AlliasColor;
            }
            else nickNameColor = GlobalSettingsLoader.Load().Colors.EnemyColor;

            nickNameTMP.color = nickNameColor;
        }

        private void OnHealthReduced(float currentHP)
        {
            frontSlider.value = health.CurrentHPInPercent;
            StopAllCoroutines();
            if(health.IsAlive)StartCoroutine(ReduceBackSlider());
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