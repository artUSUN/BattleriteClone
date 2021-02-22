using Source.Code.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class Faction : IComparable<Faction>
    {
        public int Scores { get; private set; }

        public int ID { get; private set; }
        public int Layer { get; private set; }
        public LayerMask EnemiesLayers { get; private set; }
        public int AliveUnitsCount => units.Count;


        public event Action<Unit, Unit> UnitDied;
        public event Action<int> ScoresChanged;


        private Dictionary<int, Unit> units; //Player id, Unit
        private readonly UnitSpawner unitSpawner;
        private readonly SessionSettings sessionSettings;

        public Faction(Dictionary<int, Unit> units, UnitSpawner unitSpawner)
        {
            this.units = units;
            this.unitSpawner = unitSpawner;
            sessionSettings = SessionSettings.Instance;
        }

        public void Initialize(int index)
        {
            ID = index;

            Layer = 20 + index;
            int enemiesLayers = 0;
            for (int i = 0; i < sessionSettings.Factions.Length; i++)
            {
                if (i == index) continue;
                enemiesLayers += 1 << (20 + i);
            }
            EnemiesLayers = enemiesLayers;

            var tempUnits = units;
            units = new Dictionary<int, Unit>();

            foreach (var unit in tempUnits)
            {
                unit.Value.Initialize(this, unit.Key);
                AddUnit(unit.Value);
            }
        }

        public int CompareTo(Faction f)
        {
            return this.Scores.CompareTo(f.Scores);
        }

        public void AddUnit(Unit unit)
        {
            if (units.ContainsKey(unit.OwnerPlayerID))
            {
                Debug.LogError("Unit already in unit list", unit.Transform);
                return;
            }
            units.Add(unit.OwnerPlayerID, unit);
            unit.gameObject.layer = Layer;
            unit.HealthComponent.Died += OnUnitDied;
        }

        public void AddScore(int count)
        {
            if (sessionSettings.GlobalState.Current != GlobalState.States.Game) return;

            if (count < 0)
            {
                Debug.LogError($"Can't add to Faction {ID} less than zero coins.");
                return;
            }

            Scores += count;
            ScoresChanged?.Invoke(Scores);
        }

        private void RemoveUnit(Unit unit)
        {
            if (units.ContainsKey(unit.OwnerPlayerID) == false)
            {
                Debug.LogError("Cant find unit", unit.Transform);
                return;
            }
            units.Remove(unit.OwnerPlayerID);
        }

        private void OnUnitDied(Unit who, Unit from)
        {
            UnitDied?.Invoke(who, from);
            RemoveUnit(who);
            var players = sessionSettings.SetupSettings.Players;
            var playerSettings = Array.Find(players, p => p.PlayerID == who.OwnerPlayerID);

            if (from != null)
            {
                from.Faction.AddScore(sessionSettings.SetupSettings.ScoresFromKill);
            }

            unitSpawner.RespawnUnit(playerSettings);
        }
    }
}