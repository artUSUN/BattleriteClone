using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Photon
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 10;
        [SerializeField] private float damage = 30;
        [SerializeField] private float selfDestroyAfter = 20;
        [SerializeField] private Vector3 halfSize;
        private Transform tr;
        private float selfDestroyTimer;

        private void Start()
        {
            tr = transform;
        }

        private void Update()
        {
            tr.position += tr.forward * speed * Time.deltaTime;
            var colliders = Physics.OverlapBox(tr.position, halfSize);
            if (colliders.Length != 0)
            {
                DealDamage(colliders);
                SelfDestroy();
            }

            selfDestroyTimer += Time.deltaTime;
            if (selfDestroyTimer > selfDestroyAfter)
            {
                SelfDestroy();
            }
        }

        private void SelfDestroy()
        {
            Destroy(this.gameObject, 0.1f);
        }

        private void DealDamage(Collider[] colliders)
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    var playerHP = collider.GetComponent<PlayerHP>();
                    if (playerHP != null) 
                    {
                        playerHP.ApplyDamage(damage);
                    }
                }
            }
        }
    }
}
