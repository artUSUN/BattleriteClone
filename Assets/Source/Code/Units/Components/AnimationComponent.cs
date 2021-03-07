using Photon.Pun;
using Source.Code.Environment;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class AnimationComponent : MonoBehaviour
    {
        [SerializeField] private float bodyGetDownAfter = 3f;
        [SerializeField] private float bodyGetDownSpeed = 2f;
        [SerializeField] private float bodyDestroyDelay = 2f;

        private Unit unit;
        private Animator animator;

        private int takeDamageLayerIndex, fullBodyLayerIndex;
        private float takeDamageDuration = 0.667f;
        private WaitForSeconds waitForTakeDamage;
        private Coroutine takeDamageCoroutine;

        private Transform deathAnimationModel;

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

            InitializeDeathAnimationModel();
        }

        public void OnDisable()
        {
            if (deathAnimationModel == null) return;
            deathAnimationModel.transform.position = unit.Transform.position;
            deathAnimationModel.transform.rotation = unit.Model.rotation;
            deathAnimationModel.gameObject.SetActive(true);
            var modelCopyAnimator = deathAnimationModel.GetComponentInChildren<Animator>();
            modelCopyAnimator.SetLayerWeight(fullBodyLayerIndex, 1);
            modelCopyAnimator.CrossFade("Die", 0.1f);
            deathAnimationModel.gameObject.AddComponent<Destroyer>().DestroyAfterFallingUnderground(bodyGetDownAfter, bodyGetDownSpeed, bodyDestroyDelay);
        }

        public void SpeedObserver()
        {

        }

        public void SetLegsAnimation()
        {
            Vector3 offsetDirFromLastFrame = (unit.Transform.position - unit.LastFramePosition).normalized;

            var angle = Vector2.SignedAngle(Vector2.up, new Vector2(Transform.forward.x, Transform.forward.z));
            Vector3 rot = Quaternion.Euler(0, angle, 0) * new Vector3(offsetDirFromLastFrame.x, 0, offsetDirFromLastFrame.z);

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

        private void OnTakeDamage(Vector3 fromPoint, Unit fromUnit)
        {
            if (takeDamageCoroutine != null) StopCoroutine(takeDamageCoroutine);
            if (unit.HealthComponent.IsAlive) takeDamageCoroutine = StartCoroutine(TakeDamageCoroutine(fromPoint, fromUnit));
        }

        private IEnumerator TakeDamageCoroutine(Vector3 fromPoint, Unit fromUnit)
        {
            animator.SetLayerWeight(takeDamageLayerIndex, 1);
            float angle = Vector3.SignedAngle(unit.Model.right, (fromPoint - unit.Transform.position), Vector3.up);
            string animName = angle <= 0 ? "TakeDamageBackward" : "TakeDamageForward";
            animator.Play(animName);
            yield return waitForTakeDamage;
            animator.SetLayerWeight(takeDamageLayerIndex, 0);
        }

        private void InitializeDeathAnimationModel()
        {
            deathAnimationModel = Instantiate(unit.Model.gameObject).transform;
            deathAnimationModel.gameObject.name = unit.OwnerNickName + " death animation model";
            deathAnimationModel.gameObject.SetActive(false);
            var trView = deathAnimationModel.GetComponent<PhotonTransformView>();
            if (trView != null) Destroy(trView);
        }
    }
}