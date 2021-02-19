using Source.Code.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class Faction
    {
        private UnitSpawner unitSpawner;

        public int Index { get; private set; }
        public int Layer { get; private set; }
        public LayerMask EnemiesLayers { get; private set; }


        private Dictionary<int, Unit> units; //Player id, Unit

        public Faction(Dictionary<int, Unit> units, UnitSpawner unitSpawner)
        {
            this.units = units;
            this.unitSpawner = unitSpawner;
        }

        public void Initialize(int index)
        {
            Index = index;

            var sessionSettings = SessionSettings.Instance;

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
            RemoveUnit(who);
            var players = SessionSettings.Instance.SetupSettings.Players;
            var playerSettings = Array.Find(players, p => p.PlayerID == who.OwnerPlayerID);
            unitSpawner.RespawnUnit(playerSettings);
        }
    }
}