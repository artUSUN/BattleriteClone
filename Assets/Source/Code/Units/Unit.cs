using Source.Code.PlayerInput;
using Source.Code.Units.Components;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.Units
{
    public class Unit : MonoBehaviour
    {
        public Vector2 DeltaMove { get; private set; }

        public int OwnerPlayerID { get; private set; }
        public Faction Faction { get; private set; }
        public AnimationComponent AnimationComponent { get; private set; }
        public AttackComponent AttackComponent { get; private set; }
        public Mover MoverComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public Transform Model => AnimationComponent.Transform;
        public Transform Transform { get; private set; }
        

        public void Initialize(Faction faction, int ownerID)
        {
            if (Faction != null)
            {
                Debug.Log("Unit already initialized", transform);
                return;
            }

            Faction = faction;
            OwnerPlayerID = ownerID;
        }

        public void SubscribeToEvents(PlayerInputSystem inputSystem)
        {
            inputSystem.DeltaMove += OnDeltaMove;
            inputSystem.SpacePressed += OnSpacePressed;
            inputSystem.Mouse0Pressed += OnMouse0Pressed;
            MoverComponent.SubscribeOnInput(inputSystem);
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
            MoverComponent.Run();
        }

        private void OnSpacePressed()
        {
            AttackComponent.TryDoRoll();
        }

        private void OnMouse0Pressed()
        {
            AttackComponent.TryRaiseMainAttack();
        }

        private void OnDeltaMove(Vector2 deltaMove)
        {
            DeltaMove = deltaMove;
        }
    }
}