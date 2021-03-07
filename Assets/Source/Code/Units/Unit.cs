using Photon.Pun;
using Source.Code.PlayerInput;
using Source.Code.Units.Components;
using Source.Code.Utils;
using System;
using UnityEngine;

namespace Source.Code.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Transform model;

        private Action updateTick;

        public int OwnerPlayerID { get; private set; }
        public int ActorNumber { get; private set; }
        public string OwnerNickName { get; private set; }
        public Faction Faction { get; private set; }
        public AnimationComponent AnimationComponent { get; private set; }
        public AttackComponent AttackComponent { get; private set; }
        public Mover MoverComponent { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        public Transform Model => model;
        public Transform Transform { get; private set; }
        public bool IsItControlledUnit { get; private set; } = false;
        public PhotonView PhotonView { get; private set; }
        public Vector3 LastFramePosition { get; private set; }
        public PhotonTransformViewClassic PhotonTransformView { get; private set; }

        public void SubscribeToEvents(PlayerInputSystem inputSystem)
        {
            inputSystem.SpacePressed += OnSpacePressed;
            inputSystem.Mouse0Pressed += OnMouse0Pressed;
            MoverComponent.SubscribeOnInput(inputSystem);
        }

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            PhotonTransformView = GetComponent<PhotonTransformViewClassic>();

            var sessionSettings = SessionSettings.Instance;

            PlayerSettings playerSettings = sessionSettings.SetupSettings.Players[PhotonView.CreatorActorNr];
            this.Faction = sessionSettings.Factions[playerSettings.FactionID];
            OwnerPlayerID = playerSettings.PlayerOrdinalID;
            ActorNumber = PhotonView.CreatorActorNr;

            OwnerNickName = PhotonView.Owner.NickName;
            gameObject.name = $"unit id{OwnerPlayerID} {PhotonView.Owner.NickName}";

            gameObject.layer = Faction.Layer;

            Transform = transform;

            if (Transform.localRotation != Quaternion.identity)
            {
                Model.localRotation = Transform.localRotation;
                Transform.localRotation = Quaternion.identity;
            }

            MoverComponent = GetComponent<Mover>();
            MoverComponent.Initialize(this);

            AttackComponent = GetComponent<AttackComponent>();
            AttackComponent.Initialize(this);

            HealthComponent = GetComponent<HealthComponent>();
            HealthComponent.Initialize(this);

            AnimationComponent = GetComponentInChildren<AnimationComponent>();
            AnimationComponent.Initialize(this);

            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonView.CreatorActorNr)
            {
                IsItControlledUnit = true;
                sessionSettings.SetControlledUnit(this);
            }
            else
            {
                updateTick += AnimationComponent.SpeedObserver;
            }

            Faction.AddUnit(this);

            LastFramePosition = Transform.position;
        }

        private void Update()
        {
            AnimationComponent.SetLegsAnimation();
            MoverComponent.Run();

            updateTick?.Invoke();

            LastFramePosition = Transform.position;
        }

        private void OnSpacePressed()
        {
            AttackComponent.TryDoRoll();
        }

        private void OnMouse0Pressed()
        {
            AttackComponent.TryRaiseMainAttack();
        }
    }
}