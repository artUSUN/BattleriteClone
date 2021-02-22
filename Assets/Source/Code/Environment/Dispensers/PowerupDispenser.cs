using Source.Code.Environment.Powerups;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.Environment.Dispensers
{
    public class PowerupDispenser : MonoBehaviour
    {
        [SerializeField] private bool spawnInStart = true;
        [SerializeField] private float respawnPeriod = 10;
        [SerializeField] private GameObject powerupPrefab;
        [SerializeField] private Image respawnCooldownVisualizator;

        private Transform tr;

        private void Start()
        {
            tr = transform;

            if (spawnInStart)
            {
                SpawnPowerup();
            }
            else
            {
                StartCoroutine(RespawnTimer());
            }
        }

        private void OnPowerupPickedUp()
        {
            StartCoroutine(RespawnTimer());
        }

        private void SpawnPowerup()
        {
            var powerupGO = Instantiate(powerupPrefab, tr);
            var powerup = powerupGO.GetComponent<BasePowerup>();
            if (powerup == null) 
            {
                Debug.LogError("Can't find BasePowerup in powerup prefab", powerupPrefab);
                return;
            }
            powerup.PowerupPickedUp += OnPowerupPickedUp;
        }

        private IEnumerator RespawnTimer()
        {
            float timer = 0;
            while (timer < respawnPeriod)
            {
                respawnCooldownVisualizator.fillAmount = 1 - timer / respawnPeriod;
                timer += Time.deltaTime;
                yield return null;
            }
            respawnCooldownVisualizator.fillAmount = 0;
            SpawnPowerup();
        }
    }
}