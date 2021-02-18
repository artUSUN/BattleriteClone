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

        public Vector3 lastFrameSightDirection;

        public void Initialize(Unit unit)
        {
            this.unit = unit;
            cc = GetComponent<CharacterController>();
        }

        public void Move(Vector2 direction)
        {
            direction = Vector2.ClampMagnitude(direction, 1);
            var moveSpeed = speed * new Vector3(direction.x, 0, direction.y);
            cc.SimpleMove(moveSpeed);
        }

        public void LookAt(Transform lookPivot)
        {
            Vector3 newSightDirection = lookPivot.position - unit.Transform.position;
            newSightDirection.y = 0;
            newSightDirection = newSightDirection.normalized;
            newSightDirection = Vector3.RotateTowards(unit.Model.forward, newSightDirection, rotSpeed * Time.deltaTime, 0.0f);
            unit.Model.rotation = Quaternion.LookRotation(newSightDirection);
        }
    }
}