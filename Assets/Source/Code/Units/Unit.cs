using Source.Code.Units.Components;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.Units
{
    public class Unit : MonoBehaviour
    {
        public Vector2 DeltaMove { get; set; }

        public Faction Faction { get; private set; }
        public AnimationComponent AnimationComponent { get; private set; }
        public AttackComponent AttackComponent { get; private set; }
        public Mover MoverComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public Transform Model => AnimationComponent.Transform;
        public Transform Transform { get; private set; }
        

        public void Initialize(Faction faction)
        {
            if (Faction != null)
            {
                Debug.Log("Unit already initialized", transform);
                return;
            }

            Faction = faction;
        }

        private void Awake()
        {
            Transform = transform;
            MoverComponent = GetComponent<Mover>();
            MoverComponent.Initialize(this);
            AttackComponent = GetComponent<AttackComponent>();
            AttackComponent.Initialize(this);
            HealthComponent = GetComponent<HealthComponent>();
            HealthComponent.Initialize(this);
            AnimationComponent = GetComponentInChildren<AnimationComponent>();
            AnimationComponent.Initialize(this);
        }

        private void Update()
        {
            AnimationComponent.SetLegsAnimation();
        }
    }
}