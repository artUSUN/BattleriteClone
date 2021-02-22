using Source.Code.Units;
using UnityEngine;

namespace Source.Code.Environment.Powerups
{
    public class HealPowerup : BasePowerup
    {
        [Header("Heal Settings")]
        [SerializeField] private float healPower = 50;
        [Header("Checker Settings")]
        [SerializeField] private float checkerRadius = 1;
        [SerializeField] private Transform checkerCenter;
        [Header("Heal FX")]
        [SerializeField] private GameObject unitHealedFX;
        [SerializeField] private float destroyFXDelay = 1.5f;

        private void Update()
        {
            CheckUnit();
        }

        private void CheckUnit()
        {
            var colliders = Physics.OverlapSphere(checkerCenter.position, checkerRadius, playerLayer);
            if (colliders.Length != 0)
            {
                Unit unit = colliders[0].GetComponentInChildren<Unit>();
                unit.HealthComponent.ApplyHeal(healPower);
                var fxGO = Instantiate(unitHealedFX, unit.Transform.position, Quaternion.identity, unit.Transform);
                Destroy(fxGO, destroyFXDelay);
                if (colliders.Length > 1)
                {
                    Debug.LogError("More than 1 collider finded by HealPowerup");
                }
                RaisePowerupPickedUpEvent();
                Destroy(gameObject);
            }
        }
    }
}