using Source.Code.PlayerInput;
using System.Collections;
using UnityEngine;

namespace Source.Code.Units.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed = 15f;

        private CharacterController cc;
        private Unit unit;
        private Transform lookPivot;
        private bool lockRotation = false, lockMoving = false;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            cc = GetComponent<CharacterController>();
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

        public void MakeMove(Vector3 speed, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(MakeMoveCoroutine(speed, duration));
        }

        public IEnumerator MakeMoveCoroutine(Vector3 speed, float duration)
        {
            lockMoving = true;
            lockRotation = true;

            float timer = 0;
            while (timer < duration)
            {
                cc.SimpleMove(speed);
                timer += Time.deltaTime;
                yield return null;
            }

            lockMoving = false;
            lockRotation = false;
        }
    }
}