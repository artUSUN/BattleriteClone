using Photon.Pun;
using Source.Code.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Source.Code.Utils
{
    public class Faction : IComparable<Faction>
    {
        public int Scores { get; private set; }

        public int ID { get; private set; }
        public int Layer { get; private set; }
        public LayerMask EnemiesLayers { get; private set; }
        public int AliveUnitsCount => units.Count;
        
        public event Action<int> ScoresChanged;

        private SpawnPoints spawnPoints;
        private Dictionary<int, Unit> units = new Dictionary<int, Unit>(); //Player id, Unit
        private readonly UnitSpawner unitSpawner;
        private readonly SessionSettings sessionSettings;

        public static void Initialize(SessionSettings sessionSettings, UnitSpawner unitSpawner)
        {
            int countOfFactions = sessionSettings.SetupSettings.Factions.Count;

            var factions = new Faction[countOfFactions];

            for (int i = 0; i < countOfFactions; i++)
            {
                factions[i] = new Faction(sessionSettings, unitSpawner);
            }

            sessionSettings.SetFactions(factions);
        }

        public Faction(SessionSettings sessionSettings, UnitSpawner unitSpawner)
        {
            this.unitSpawner = unitSpawner;
            this.sessionSettings = sessionSettings;
            
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

            spawnPoints = unitSpawner.FactionsSpawnPoints[index];
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
            
            unit.HealthComponent.Died += OnUnitDied;
        }

        public void SetScore(int count)
        {
            if (sessionSettings.GlobalState.Current != GlobalState.States.Game) return;

            if (count < 0)
            {
                Debug.LogError($"Can't set to Faction {ID} less than zero coins.");
                return;
            }

            if (Scores == count) return;

            Scores = count;
            ScoresChanged?.Invoke(Scores);
        }

        public Transform GetControlledUnitSpawnZone(PlayerSettings playerSettings)
        {
            var players = sessionSettings.SetupSettings.Factions[ID];
            return spawnPoints.Transforms[players.IndexOf(playerSettings)];
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

        private void OnUnitDied(Unit who)
        {
            RemoveUnit(who);

            int localPlayerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

            if (localPlayerActorNr == who.ActorNumber)
            {
                var playerSettings = sessionSettings.SetupSettings.Players[localPlayerActorNr];
                Transform spawnPoint = GetControlledUnitSpawnZone(playerSettings);
                unitSpawner.RespawnUnit(playerSettings, spawnPoint);
            }
        }
    }
}