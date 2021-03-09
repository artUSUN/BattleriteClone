using ExitGames.Client.Photon;
using Photon.Pun;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using Source.Code.Units.Bars;
using Source.Code.Utils;
using System;
using UnityEngine;

namespace Source.Code.Units.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private Transform hpBarPlace;
        [SerializeField] private float maxHP = 100;

        private Unit unit;
        private HPBar hpBar;
        private bool isDied = false;

        public float CurrentHP { get; private set; }
        public float CurrentHPInPercent => CurrentHP / maxHP;
        public bool IsAlive => !isDied;

        public event Action<float> HealthReduced;
        public event Action<float> HealthIncreased;
        public event Action<Vector3, Unit> TakedDamage;
        public event Action<Unit> Died;

        public void Initialize(Unit unit)
        {
            var globalSettings = GlobalSettingsLoader.Load();
            this.unit = unit;
            CurrentHP = maxHP;
            hpBar = Instantiate(globalSettings.Prefabs.UnitBar, hpBarPlace.position, Quaternion.Euler(70, 0, 0), unit.Transform).GetComponent<HPBar>();
            hpBar.Initialize(this, unit);
        }

        private void OnDisable()
        {
            Debug.Log("On Disable in Health Component");
            if (SessionSettings.Instance.ControlledUnit != unit)
            {
                Died?.Invoke(unit);
            }
        }

        public void ApplyDamage(float value, Vector3 fromPoint, Unit fromUnit)
        {
            if (value < 0)
            {
                Debug.Log("Damage cannot be less than 0");
                return;
            }

            if (unit.PhotonView.IsMine)
            {
                unit.PhotonView.RPC("ApplyDamageRemotely", RpcTarget.Others, value, fromPoint);
            }

            CurrentHP -= value;
            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
                Death(fromUnit.Faction);
            }

            HealthReduced?.Invoke(CurrentHP);
            TakedDamage?.Invoke(fromPoint, fromUnit);
        }

        [PunRPC]
        private void ApplyDamageRemotely(float value, Vector3 fromPoint, PhotonMessageInfo info)
        {
            var fromPlayer = info.Sender;
            Unit fromUnit = null;
            Faction fromFaction = null;
            if (info.Sender != null)
            {
                var sessionSettigns = SessionSettings.Instance;
                PlayerSettings playerSettings = sessionSettigns.SetupSettings.Players[fromPlayer.ActorNumber];
                fromFaction = sessionSettigns.Factions[playerSettings.FactionID];
            }

            CurrentHP -= value;
            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
                Death(fromFaction);
            }

            HealthReduced?.Invoke(CurrentHP);
            TakedDamage?.Invoke(fromPoint, fromUnit);
        }

        public void ApplyHeal(float value)
        {
            if (value < 0)
            {
                Debug.Log("Heal cannot be less than 0");
                return;
            }

            if (unit.PhotonView.IsMine)
            {
                unit.PhotonView.RPC("ApplyHealRemotely", RpcTarget.Others, value);
            }

            CurrentHP += value;
            if (CurrentHP > maxHP) CurrentHP = maxHP;

            HealthIncreased?.Invoke(CurrentHP);
        }

        [PunRPC]
        private void ApplyHealRemotely(float value, PhotonMessageInfo info)
        {
            CurrentHP += value;
            if (CurrentHP > maxHP) CurrentHP = maxHP;

            HealthIncreased?.Invoke(CurrentHP);
        }

        private void Death(Faction fromFaction)
        {
            if (isDied) return;
            isDied = true;

            if (fromFaction != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    
                    string factionScoresKey = GlobalConst.GetFactionScoresKey(fromFaction.ID);
                    int score = 
                        SessionSettings.Instance.SetupSettings.ScoresFromKill + 
                        PhotonExtensions.GetValueOrReturnDefault<int>(PhotonNetwork.CurrentRoom.CustomProperties, factionScoresKey);
                    Hashtable props = new Hashtable { { factionScoresKey, score } };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }
            }

            if (SessionSettings.Instance.ControlledUnit == unit)
            {
                Died?.Invoke(unit);
            }

            PhotonNetwork.Destroy(unit.gameObject);
        }
    }
}