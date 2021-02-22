using Source.Code.Units.Bars;
using Source.Code.Utils;
using System;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private Transform hpBarPlace;
        [SerializeField] private float maxHP = 100;

        private Unit unit;
        private HPBar hpBar;
        private bool isDied = false;

        public float CurrentHP { get; private set; }
        public float CurrentHPInPercent => CurrentHP / maxHP;

        public event Action<float> HealthReduced;
        public event Action<float> HealthIncreased;
        public event Action<Vector3, Unit> TakedDamage;
        public event Action<Unit, Unit> Died;

        public void Initialize(Unit unit)
        {
            var globalSettings = GlobalSettingsLoader.Load();
            this.unit = unit;
            CurrentHP = maxHP;
            hpBar = Instantiate(globalSettings.Prefabs.UnitBar, hpBarPlace.position, Quaternion.Euler(70, 0, 0), unit.Transform).GetComponent<HPBar>();
            hpBar.Initialize(this);
        }

        public void ApplyDamage(float value, Vector3 fromPoint, Unit fromUnit)
        {
            if (value < 0)
            {
                Debug.Log("Damage cannot be less than 0");
                return;
            }

            CurrentHP -= value;
            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
                Death(fromUnit);
            }

            HealthReduced?.Invoke(CurrentHP);
            TakedDamage?.Invoke(fromPoint, fromUnit);
        }

        public void ApplyHeal(float value)
        {
            if (value < 0)
            {
                Debug.Log("Heal cannot be less than 0");
                return;
            }

            CurrentHP += value;
            if (CurrentHP > maxHP) CurrentHP = maxHP;

            HealthIncreased?.Invoke(CurrentHP);
        }

        private void Death(Unit from)
        {
            if (isDied) return;
            isDied = true;
            Died?.Invoke(unit, from);
            Destroy(unit.gameObject);
        }
    }
}