using Source.Code.Environment.Missle;
using System;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [Header("Main Attack Settigns")]
        [SerializeField] private float damage = 34;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float delay = 0.1f;
        [SerializeField] private float missleLifeTime = 1f;
        [SerializeField] private float missleSpeed = 3f;
        [SerializeField] private GameObject misslePrefab;
        [SerializeField] private Transform missleCreatePoint;
        [Header("Roll settings")]
        [SerializeField] private float rollSpeed = 10;
        [SerializeField] private float rollDuration = 0.5f;
        [SerializeField] private float rollCooldown = 8f;

        private Unit unit;
        private WaitForSeconds rollEndWaiter;
        private AbilityCooldown roll, mainAttack;
        private bool isAbilitiesLocked = false;

        public event Action MainAttackEvent;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            mainAttack = new AbilityCooldown(attackCooldown);
            roll = new AbilityCooldown(rollCooldown);
            rollEndWaiter = new WaitForSeconds(rollDuration);
        }

        public void TryRaiseMainAttack()
        {
            if (mainAttack.IsInCooldown || isAbilitiesLocked) return;
            DoMainAttack();
            StartCoroutine(WaitForCooldown(mainAttack));
        }

        public void TryDoRoll()
        {
            if (roll.IsInCooldown || isAbilitiesLocked) return;
            StartCoroutine(DoRoll());
            StartCoroutine(WaitForCooldown(roll));
        }

        private IEnumerator DoRoll()
        {
            unit.AnimationComponent.PlayRollAnimation();
            unit.MoverComponent.MakeMove(unit.Model.forward * rollSpeed, rollDuration);

            yield return rollEndWaiter;

            unit.AnimationComponent.EndRollAnimation();
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

        

        private IEnumerator WaitForCooldown(AbilityCooldown ability)
        {
            ability.IsInCooldown = true;
            yield return ability.Waiter;
            ability.IsInCooldown = false;
        }
    }

    internal class AbilityCooldown
    {
        internal WaitForSeconds Waiter { get; private set; }
        internal bool IsInCooldown = false;

        internal AbilityCooldown(float cooldownDuration)
        {
            Waiter = new WaitForSeconds(cooldownDuration);
        }
    }
}