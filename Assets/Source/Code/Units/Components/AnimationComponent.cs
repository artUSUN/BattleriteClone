using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class AnimationComponent : MonoBehaviour
    {
        [SerializeField] private float bodyGetDownAfter = 3f;
        [SerializeField] private float bodyGetDownSpeed = 2f;
        [SerializeField] private float destroyDelay = 2f;

        private Unit unit;
        private Animator animator;

        private int takeDamageLayerIndex, fullBodyLayerIndex;
        private float takeDamageDuration = 0.667f;
        private WaitForSeconds waitForTakeDamage;
        private Coroutine takeDamageCoroutine;

        public Transform Transform { get; private set; }

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            animator = GetComponentInChildren<Animator>();
            if (!animator) Debug.LogError("Can't find animator", transform);
            Transform = transform;

            takeDamageLayerIndex = animator.GetLayerIndex("TakeDamageLayer");
            fullBodyLayerIndex = animator.GetLayerIndex("FullBody");
            waitForTakeDamage = new WaitForSeconds(takeDamageDuration);

            unit.HealthComponent.TakedDamage += OnTakeDamage;
            unit.HealthComponent.Died += OnDied;
        }

        public void SetLegsAnimation()
        {
            var angle = Vector2.SignedAngle(Vector3.up, new Vector2(Transform.forward.x, Transform.forward.z));
            Vector3 rot = Quaternion.Euler(0, angle, 0) * new Vector3(unit.DeltaMove.x, 0, unit.DeltaMove.y);

            animator.SetFloat("MoveDeltaX", rot.x);
            animator.SetFloat("MoveDeltaY", rot.z);
        }


        public void PlayAnimation(string name)
        {
            animator.CrossFade(name, 0.1f);
        }

        public void PlayRollAnimation()
        {
            animator.SetLayerWeight(fullBodyLayerIndex, 1);
            PlayAnimation("Roll");
        }

        public void EndRollAnimation()
        {
            animator.SetLayerWeight(fullBodyLayerIndex, 0);
            PlayAnimation("Idle");
        }

        private void OnDied(Unit whoDies, Unit from)
        {
            animator.SetLayerWeight(fullBodyLayerIndex, 1);
            PlayAnimation("Die");
            unit.Model.SetParent(unit.Transform.parent);
            StartCoroutine(DieCoroutine());
        }

        private IEnumerator DieCoroutine()
        {
            yield return new WaitForSeconds(bodyGetDownAfter);
            var tr = transform;
            float timer = 0;
            while (timer < destroyDelay)
            {
                tr.Translate(Vector3.down * bodyGetDownSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnTakeDamage(Vector3 fromPoint, Unit fromUnit)
        {
            if (takeDamageCoroutine != null) StopCoroutine(takeDamageCoroutine);
            takeDamageCoroutine = StartCoroutine(TakeDamageCoroutine(fromPoint, fromUnit));
        }

        private IEnumerator TakeDamageCoroutine(Vector3 fromPoint, Unit fromUnit)
        {
            animator.Play("TakeDamage");
            animator.SetLayerWeight(takeDamageLayerIndex, 1);
            float angle = Vector3.SignedAngle(unit.Model.forward, (fromPoint - unit.Transform.position), Vector3.up);
            int takeDamageSide = angle <= 0 ? 0 : 1;
            animator.SetFloat("TakeDamageSide", takeDamageSide);
            yield return waitForTakeDamage;
            animator.SetLayerWeight(takeDamageLayerIndex, 0);
        }
    }
}