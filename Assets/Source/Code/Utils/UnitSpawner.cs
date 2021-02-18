using Source.Code.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPoints[] spawnPoints;

        private bool spawned = false;
        private Transform[] factionsRoot;
        private Dictionary<int, Transform> spawnPointsByPlayerID;

        public Faction[] InitSpawn(SessionSetup setupSettings)
        {
            if (spawned) 
            {
                Debug.LogError("Units already spawn");
                return null;
            }

            spawned = true;

            Dictionary<int, List<PlayerSettings>> unitsByFaction = new Dictionary<int, List<PlayerSettings>>();

            foreach (var player in setupSettings.Players)
            {
                if (unitsByFaction.ContainsKey(player.FactionID))
                {
                    unitsByFaction[player.FactionID].Add(player);
                }
                else
                {
                    unitsByFaction.Add(player.FactionID, new List<PlayerSettings> { player } );
                }
            }

            if (unitsByFaction.Count > spawnPoints.Length) Debug.LogException(new Exception("Not enough spawn points"), transform);


            spawnPointsByPlayerID = new Dictionary<int, Transform>();

            Faction[] factions = new Faction[unitsByFaction.Count];

            for (int i = 0; i < unitsByFaction.Count; i++)
            {
                if (unitsByFaction[i].Count > spawnPoints[i].Points.Length) Debug.LogException(new Exception($"Not enough spawn points. i = {i}"), transform);

                Dictionary<Transform, Unit> createdUnits = new Dictionary<Transform, Unit>();

                factionsRoot = new Transform[factions.Length];


                var newEmptyGO = new GameObject($"Faction {i}");
                factionsRoot[i] = newEmptyGO.transform;

                for (int j = 0; j < unitsByFaction[i].Count; j++)
                {
                    spawnPointsByPlayerID.Add(unitsByFaction[i][j].PlayerID, spawnPoints[i].Points[j]);

                    var newUnitTransform = 
                        Instantiate(unitsByFaction[i][j].UnitPrefab, spawnPoints[i].Points[j].position, spawnPoints[i].Points[j].rotation, factionsRoot[i]).transform;
                    var unitScript = newUnitTransform.GetComponentInChildren<Unit>();
                    createdUnits.Add(newUnitTransform, unitScript);
                    if (unitsByFaction[i][j].PlayerID == SessionSettings.Instance.CurrentPlayerID)
                    {
                        if (unitScript == null) Debug.LogError("Unit is null", transform);
                        else SessionSettings.Instance.InitControlledUnit(unitScript);
                    }
                }

                factions[i] = new Faction(createdUnits);
            }

            return factions;
        } 

        public void RespawnUnit(PlayerSettings settings)
        {
            var spawnPoint = spawnPointsByPlayerID[settings.PlayerID];

            var newUnitTransform =
                Instantiate(settings.UnitPrefab, spawnPoint.position, spawnPoint.rotation, factionsRoot[settings.FactionID]).transform;
            var unitScript = newUnitTransform.GetComponentInChildren<Unit>();
            SessionSettings.Instance.Factions[settings.FactionID].AddUnit(unitScript);
        }
    }

    [Serializable]
    public class SpawnPoints
    {
        [SerializeField] private Transform[] points;

        public Transform[] Points => points;
    }

}