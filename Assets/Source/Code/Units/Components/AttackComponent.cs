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
        [SerializeField] private float attackDelay = 0.1f;
        [SerializeField] private float missleLifeTime = 1f;
        [SerializeField] private float missleSpeed = 3f;
        [SerializeField] private GameObject misslePrefab;
        [SerializeField] private Transform missleCreatePoint;

        private GlobalSettings globalSettings;
        private Unit unit;
        private WaitForSeconds rollEndWaiter, attackWaiter;
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
            attackWaiter = new WaitForSeconds(attackDelay);
        }

        public void TryRaiseMainAttack()
        {
            if (mainAttack.IsInCooldown || isAbilitiesLocked) return;

            Vector2 pos = new Vector2(missleCreatePoint.position.x, missleCreatePoint.position.z);
            Vector2 dir = new Vector2(unit.Model.forward.x, unit.Model.forward.z);

            StartCoroutine(DoMainAttackCoroutine(pos, dir, 0));
            unit.PhotonView.RPC("DoMainAttack", RpcTarget.Others, pos, dir);
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

            if (lag != 0)
            {
                if (lag >= duration) yield break;
                float durationMinusLag = duration - lag;
                rollEndWaiter = new WaitForSeconds(durationMinusLag);
            }

            unit.AnimationComponent.PlayRollAnimation();
            unit.MoverComponent.DoRoll(fromPosition, direction, lag);

            yield return rollEndWaiter;

            unit.AnimationComponent.EndRollAnimation();
        }

        [PunRPC]
        private void DoMainAttack(Vector2 position, Vector2 direction, PhotonMessageInfo info)
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            StartCoroutine(DoMainAttackCoroutine(position, direction, lag));
        }

        private IEnumerator DoMainAttackCoroutine(Vector2 position, Vector2 direction, float lag)
        {
            Vector3 spawnPosition = new Vector3(position.x, missleCreatePoint.position.y, position.y);
            Vector3 directionVector3 = new Vector3(direction.x, 0, direction.y);

            unit.AnimationComponent.PlayAnimation("MainAttack");

            if (lag == 0) yield return attackWaiter;
            else if (lag < attackDelay) yield return new WaitForSeconds(attackDelay - lag);
            else
            {
                spawnPosition += directionVector3 * lag;
            }
            var missle = Instantiate(misslePrefab, spawnPosition, Quaternion.identity);
            float missleLifeTimeWithLag = missleLifeTime - (lag - attackDelay);
            missle.GetComponent<LineFlyingMissle>().Initialize(unit, directionVector3, missleSpeed, missleLifeTimeWithLag, unit.Faction.EnemiesLayers, damage);
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