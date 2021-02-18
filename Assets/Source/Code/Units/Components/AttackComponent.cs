using Source.Code.Environment.Missle;
using System;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float damage = 34;
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private float delay = 0.1f;
        [SerializeField] private float missleLifeTime = 1f;
        [SerializeField] private float missleSpeed = 3f;
        [SerializeField] private GameObject misslePrefab;
        [SerializeField] private Transform missleCreatePoint;

        private bool inCooldown = false;
        private WaitForSeconds cooldownWaiter;
        private Unit unit;

        public event Action MainAttackEvent;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            cooldownWaiter = new WaitForSeconds(cooldown);
        }

        public void TryRaiseMainAttack()
        {
            if (inCooldown) return;
            inCooldown = true;
            DoMainAttack();
            StartCoroutine(WaitForCooldown());
        }

        private void DoMainAttack()
        {
            unit.AnimationComponent.PlayAnimation("MainAttack");
            Invoke(nameof(InstantiatePrefab), delay);
        }

        private void InstantiatePrefab()
        {
            var missle = Instantiate(misslePrefab, missleCreatePoint.position, Quaternion.identity);
            missle.GetComponent<LineFlyingMissle>().Initialize(unit, unit.Model.forward, missleSpeed, missleLifeTime, unit.Faction.EnemiesLayers, damage);
        }

        private IEnumerator WaitForCooldown()
        {
            yield return cooldownWaiter;
            inCooldown = false;
        }
    }
}