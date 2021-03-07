using Source.Code.PlayerInput;
using Source.Code.Utils;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed = 15f;

        private GlobalSettings globalSettings;
        private CharacterController cc;
        private Unit unit;
        private Transform lookPivot;
        private bool lockRotation = false, lockMoving = false;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            cc = GetComponent<CharacterController>();
            globalSettings = GlobalSettingsLoader.Load();
        }

        public void SubscribeOnInput(PlayerInputSystem inputSystem)
        {
            inputSystem.DeltaMove += Move;
            lookPivot = inputSystem.LookPivot.Transform;
        }

        public void Run()
        {
            if (lookPivot != null) LookAtPivot();
        }

        private void Move(Vector2 direction)
        {
            if (direction == Vector2.zero);
            if (lockMoving) return;
            direction = Vector2.ClampMagnitude(direction, 1);
            var moveSpeed = speed * new Vector3(direction.x, 0, direction.y);
            cc.SimpleMove(moveSpeed);
        }

        private void LookAtPivot()
        {
            if (lockRotation) return;
            Vector3 newSightDirection = lookPivot.position - unit.Transform.position;
            newSightDirection.y = 0;
            newSightDirection = newSightDirection.normalized;
            newSightDirection = Vector3.RotateTowards(unit.Model.forward, newSightDirection, rotSpeed * Time.deltaTime, 0.0f);
            unit.Model.rotation = Quaternion.LookRotation(newSightDirection);
        }

        public void DoRoll(Vector2 fromPosition, Vector2 direction, float durationMinusLag)
        {
            StopAllCoroutines();
            StartCoroutine(DoRollCoroutine(fromPosition, direction, durationMinusLag));
        }

        public IEnumerator DoRollCoroutine(Vector2 fromPosition, Vector2 direction, float durationMinusLag)
        {
            float rollDuration = globalSettings.RollAbility.Duration;
            float rollSpeed = globalSettings.RollAbility.Speed;
            unit.PhotonTransformView.enabled = false;

            Debug.Log("durationMinusLag = " + durationMinusLag);

            lockMoving = true;
            lockRotation = true;

            Vector3 currentPos = unit.Transform.position;
            Vector3 targetPos = 
                new Vector3(fromPosition.x, unit.Transform.position.y, fromPosition.y) + new Vector3(direction.x, 0, direction.y) * (rollDuration * rollSpeed);

            float timer = 0;
            while (timer < durationMinusLag)
            {
                unit.Transform.position = Vector3.Lerp(currentPos, targetPos, timer / durationMinusLag);
                timer += Time.deltaTime;
                yield return null;
            }

            unit.Transform.position = targetPos;

            unit.PhotonTransformView.enabled = true;
            lockMoving = false;
            lockRotation = false;
        }
    }
}