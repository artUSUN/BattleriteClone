using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace Source.Code.Utils
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPoints[] factionsSpawnPoints;

        private WaitForSeconds respawnWaiter;
        private SessionSettings sessionSettings;

        public SpawnPoints[] FactionsSpawnPoints => factionsSpawnPoints;

        public void Initialize(SessionSettings sessionSettings)
        {
            this.sessionSettings = sessionSettings;
            respawnWaiter = new WaitForSeconds(sessionSettings.SetupSettings.RespawnDuration);
        }

        public void SpawnUnit(PlayerSettings player, Transform spawnPoint)
        {
            PhotonNetwork.Instantiate(player.UnitPrefabName, spawnPoint.position, spawnPoint.rotation);
        }

        public void RespawnUnit(PlayerSettings player, Transform spawnPoint)
        {
            StartCoroutine(RespawnCoroutine(player, spawnPoint));
        }

        private IEnumerator RespawnCoroutine(PlayerSettings player, Transform spawnPoint)
        {
            if (respawnWaiter == null)
            {
                respawnWaiter = new WaitForSeconds(sessionSettings.SetupSettings.RespawnDuration);
            }

            yield return respawnWaiter;

            SpawnUnit(player, spawnPoint);
        }
    }

    [Serializable]
    public class SpawnPoints
    {
        [SerializeField] private Transform[] transforms;

        public Transform[] Transforms => transforms;
    }
}