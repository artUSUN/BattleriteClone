using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class AnimationComponent : MonoBehaviour
    {
        private Unit unit;
        private Animator animator;

        private int takeDamageLayerIndex;
        private float takeDamageDuration = 0.667f;
        private WaitForSeconds waitForTakeDamage;

        public Transform Transform { get; private set; }

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            animator = GetComponentInChildren<Animator>();
            if (!animator) Debug.LogError("Can't find animator", transform);
            Transform = transform;

            takeDamageLayerIndex = animator.GetLayerIndex("TakeDamageLayer");
            waitForTakeDamage = new WaitForSeconds(takeDamageDuration);

            unit.HealthComponent.TakedDamage += OnTakeDamage;
        }

        public void SetLegsAnimation()
        {
            var angle = Vector2.SignedAngle(Vector3.up, new Vector2(Transform.forward.x, Transform.forward.z));
            Vector3 rot = Quaternion.Euler(0, angle, 0) * new Vector3(unit.DeltaMove.x, 0, unit.DeltaMove.y);

            animator.SetFloat("MoveDeltaX", rot.x);
            animator.SetFloat("MoveDeltaY", rot.z);
        }

        public void OnTakeDamage(Vector3 fromPoint, Unit fromUnit)
        {
            StopAllCoroutines();
            StartCoroutine(TakeDamageCoroutine(fromPoint, fromUnit));
        }

        public void PlayAnimation(string name)
        {
            animator.CrossFade(name, 0.1f);
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