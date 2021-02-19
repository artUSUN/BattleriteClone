using Source.Code.Units;
using System;
using System.Collections;
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
        private WaitForSeconds respawnWaiter;
        private SessionSettings sessionSettings;

        public void InitSpawn(SessionSetup setupSettings)
        {
            sessionSettings = SessionSettings.Instance;

            if (spawned) 
            {
                Debug.LogError("Units already spawn");
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


            spawnPointsByPlayerID = new Dictionary<int, Transform>();

            Faction[] factions = new Faction[unitsByFaction.Count];

            for (int i = 0; i < unitsByFaction.Count; i++)
            {
                if (unitsByFaction[i].Count > spawnPoints[i].Points.Length) Debug.LogException(new Exception($"Not enough spawn points. i = {i}"), transform);

                Dictionary<int, Unit> createdUnits = new Dictionary<int, Unit>();

                factionsRoot = new Transform[factions.Length];

                var newEmptyGO = new GameObject($"Faction {i}");
                factionsRoot[i] = newEmptyGO.transform;

                for (int j = 0; j < unitsByFaction[i].Count; j++)
                {
                    spawnPointsByPlayerID.Add(unitsByFaction[i][j].PlayerID, spawnPoints[i].Points[j]);

                    var newUnitTransform = 
                        Instantiate(unitsByFaction[i][j].UnitPrefab, spawnPoints[i].Points[j].position, spawnPoints[i].Points[j].rotation, factionsRoot[i]).transform;
                    var unitScript = newUnitTransform.GetComponentInChildren<Unit>();
                    createdUnits.Add(unitsByFaction[i][j].PlayerID, unitScript);
                    if (unitsByFaction[i][j].PlayerID == sessionSettings.CurrentPlayerID)
                    {
                        if (unitScript == null) Debug.LogError("Unit is null", transform);
                        else sessionSettings.SetControlledUnit(unitScript);
                    }
                }

                factions[i] = new Faction(createdUnits, this);
            }

            sessionSettings.SetFactions(factions);
        } 

        public void RespawnUnit(PlayerSettings playerSettings)
        {
            Debug.Log("Respawn unit " + playerSettings.PlayerID);
            StartCoroutine(RespawnCoroutine(playerSettings));
        }

        private IEnumerator RespawnCoroutine(PlayerSettings playerSettings)
        {
            if (respawnWaiter == null)
            {
                respawnWaiter = new WaitForSeconds(sessionSettings.SetupSettings.RespawnDuration);
            }

            yield return respawnWaiter;

            var spawnPoint = spawnPointsByPlayerID[playerSettings.PlayerID];

            var newUnitTransform =
                Instantiate(playerSettings.UnitPrefab, spawnPoint.position, spawnPoint.rotation, factionsRoot[playerSettings.FactionID]).transform;
            var unitScript = newUnitTransform.GetComponentInChildren<Unit>();
            unitScript.Initialize(sessionSettings.Factions[playerSettings.FactionID], playerSettings.PlayerID);

            sessionSettings.Factions[playerSettings.FactionID].AddUnit(unitScript);
            if (sessionSettings.CurrentPlayerID == playerSettings.PlayerID) sessionSettings.SetControlledUnit(unitScript);
        }
    }

    [Serializable]
    public class SpawnPoints
    {
        [SerializeField] private Transform[] points;

        public Transform[] Points => points;
    }
}