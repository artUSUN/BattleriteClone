using Photon.Pun;
using Source.Code.Environment.Missle;
using Source.Code.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    [DisallowMultipleComponent]
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

        private GlobalSettings globalSettings;
        private Unit unit;
        private WaitForSeconds rollEndWaiter;
        private AbilityCooldown roll, mainAttack;
        private bool isAbilitiesLocked = false;

        public event Action MainAttackEvent;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            mainAttack = new AbilityCooldown(attackCooldown);
            globalSettings = GlobalSettingsLoader.Load();
            roll = new AbilityCooldown(globalSettings.RollAbility.Cooldown);
            rollEndWaiter = new WaitForSeconds(globalSettings.RollAbility.Duration);
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
            Vector2 pos = new Vector2(unit.Transform.position.x, unit.Transform.position.z);
            Vector2 dir = new Vector2(unit.Model.forward.x, unit.Model.forward.z);

            StartCoroutine(DoRollCoroutine(pos, dir, 0));
            unit.PhotonView.RPC("DoRoll", RpcTarget.Others, pos, dir);
            StartCoroutine(WaitForCooldown(roll));
        }

        [PunRPC]
        private void DoRoll(Vector2 fromPosition, Vector2 direction, PhotonMessageInfo info)
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            StartCoroutine(DoRollCoroutine(fromPosition, direction, lag));
        }

        private IEnumerator DoRollCoroutine(Vector2 fromPosition, Vector2 direction, float lag)
        {
            float duration = GlobalSettingsLoader.Load().RollAbility.Duration;
            float durationMinusLag = duration;

            if (lag != 0)
            {
                
                if (lag >= duration) yield break;
                durationMinusLag = duration - lag;
                rollEndWaiter = new WaitForSeconds(durationMinusLag);
            }

            unit.AnimationComponent.PlayRollAnimation();
            unit.MoverComponent.DoRoll(fromPosition, direction, durationMinusLag);

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