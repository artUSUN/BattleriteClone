using Source.Code.Utils;
using System;
using UnityEngine;

namespace Source.Code.Environment.Powerups
{
    public abstract class BasePowerup : MonoBehaviour
    {
        protected LayerMask playerLayer;

        public event Action PowerupPickedUp;

        private void Start()
        {
            playerLayer = SessionSettings.Instance.AllFactionLayers;
            OverridableStart();
        }

        protected virtual void OverridableStart()
        { }

        protected void RaisePowerupPickedUpEvent()
        {
            PowerupPickedUp?.Invoke();
        }
    }
}