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

        private float standartSpeed;
        public bool lockRotation = false, lockMoving = false;

        public Vector3 lastFrameSightDirection;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            cc = GetComponent<CharacterController>();
            standartSpeed = speed;
        }

        public void Move(Vector2 direction)
        {
            if (lockMoving) return;
            direction = Vector2.ClampMagnitude(direction, 1);
            var moveSpeed = speed * new Vector3(direction.x, 0, direction.y);
            cc.SimpleMove(moveSpeed);
        }

        public void LookAt(Transform lookPivot)
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