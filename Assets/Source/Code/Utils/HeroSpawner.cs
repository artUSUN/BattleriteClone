using Source.Code.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Utils
{
    public class HeroSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPoints[] spawnPoints;

        private bool spawned = false;

        public void Spawn(SessionSetup setupSettings)
        {
            if (spawned) 
            {
                Debug.LogError("Heroes already spawn");
                return;
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


            Faction[] factions = new Faction[unitsByFaction.Count];

            for (int i = 0; i < unitsByFaction.Count; i++)
            {
                if (unitsByFaction[i].Count > spawnPoints[i].Points.Length) Debug.LogException(new Exception($"Not enough spawn points. i = {i}"), transform);

                Dictionary<Transform, Unit> createdUnits = new Dictionary<Transform, Unit>();

                var newEmptyGO = new GameObject($"Faction {i}");

                for (int j = 0; j < unitsByFaction[i].Count; j++)
                {
                    var newUnitTransform = 
                        Instantiate(unitsByFaction[i][j].UnitPrefab, spawnPoints[i].Points[j].position, spawnPoints[i].Points[j].rotation, newEmptyGO.transform).transform;
                    var unitSctipt = newUnitTransform.GetComponentInChildren<Unit>();
                    createdUnits.Add(newUnitTransform, unitSctipt);
                    if (unitsByFaction[i][j].PlayerID == SessionSettings.Instance.CurrentPlayerID)
                    {
                        var unitScript = newUnitTransform.GetComponent<Unit>();
                        if (unitScript == null) Debug.LogError("Unit is null", transform);
                        else SessionSettings.Instance.InitControlledUnit(unitScript);
                    }
                }

                factions[i] = new Faction(createdUnits);
            }

            SessionSettings.Instance.InitFactions(factions);
        } 
    }

    [Serializable]
    public class SpawnPoints
    {
        [SerializeField] private Transform[] points;

        public Transform[] Points => points;
    }

}