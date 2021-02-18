using Source.Code.Environment.Missle;
using Source.Code.Utils;
using System.Collections;
using UnityEngine;

namespace Source.Code.Environment.Test
{
    public class ShotDispenser : MonoBehaviour
    {
        [SerializeField] private float timeBetweenShots = 1.2f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float missleSpeed = 12f;
        [SerializeField] private float missleLifeTime = 3f;
        [SerializeField] private GameObject misslePrefab;
        [SerializeField] private Transform missleCreatePoint;

        private WaitForSeconds waiter;
        private Transform tr;
        private LayerMask whoIsEnemy;

        private void Awake()
        {
            tr = transform;
            waiter = new WaitForSeconds(timeBetweenShots);
            
        }

        private void Start()
        {
            whoIsEnemy = 1 << SessionSettings.Instance.ControlledUnit.Faction.Layer;
            StartCoroutine(Dispencer());
        }

        private IEnumerator Dispencer()
        {
            while (true)
            {
                yield return waiter;
                var missle = Instantiate(misslePrefab, missleCreatePoint.position, Quaternion.identity);
                missle.GetComponent<LineFlyingMissle>().Initialize(null, tr.forward, missleSpeed, missleLifeTime, whoIsEnemy, damage);
            }
        }
    }
}