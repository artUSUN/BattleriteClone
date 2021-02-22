using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.Environment
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 speed;

        private Transform tr;

        private void Start()
        {
            tr = transform;
        }

        private void Update()
        {
            tr.localRotation *= Quaternion.Euler(speed);
        }
    }
}