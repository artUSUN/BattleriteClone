using Source.Code.Units;
using Source.Code.Utils;
using System.Collections;
using UnityEngine;

namespace Source.Code.Environment.Missle
{
    public class LineFlyingMissle : MonoBehaviour
    {
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private GameObject expPrefab;

        private Unit owner;
        private Vector3 direction;
        private float speed, lifeTime, damage;
        private Transform tr;
        private LayerMask whoIsEnemies;
        private LayerMask collideWith;

        private void Awake()
        {
            tr = transform; 
        }

        public void Initialize(Unit owner, Vector3 direction, float speed, float lifeTime, LayerMask targetLayer, float damage)
        {
            this.owner = owner;
            this.direction = direction;
            this.lifeTime = lifeTime;
            this.speed = speed;
            whoIsEnemies = targetLayer;
            this.damage = damage;
            collideWith = GlobalSettingsLoader.Load().Layers.Walls.value + whoIsEnemies;
            StartCoroutine(SelfDestroyer());
        }

        private void Update()
        {
            if (gameObject.activeSelf == false) return;

            tr.Translate(direction * speed * Time.deltaTime);
            CheckTarget();
        }

        private void CheckTarget()
        {
            var colliders = Physics.OverlapSphere(tr.position, radius, collideWith, QueryTriggerInteraction.Ignore);
            if (colliders.Length != 0)
            {
                foreach (var collider in colliders)
                {
                    var unit = collider.GetComponent<Unit>();
                    if (unit != null) unit.HealthComponent.ApplyDamage(damage, tr.position, owner);
                }

                var expGO = Instantiate(expPrefab, tr.position + direction * radius, Quaternion.identity);
                Destroy(expGO, lifeTime);
                Destroy(this.gameObject);
            }
        }

        private IEnumerator SelfDestroyer()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(this.gameObject);
        }
    }
}