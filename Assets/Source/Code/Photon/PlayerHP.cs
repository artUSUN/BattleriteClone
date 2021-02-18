using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.Photon
{
    public class PlayerHP : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private float maxHP = 100f;
        [SerializeField] private Slider slider;
        private float currentHP;

        private void Start()
        {
            currentHP = maxHP;
            ChangeSliderValue();
        }

        public void ApplyDamage(float value)
        {
            if (value < 0) return;

            currentHP -= value;
            ChangeSliderValue();

            if (currentHP <= 0)
            {
                currentHP = 0;
                Death();
            }
        }

        private void Death()
        {
            
        }

        private void ChangeSliderValue()
        {
            slider.value = currentHP / maxHP;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentHP);
            }
            else
            {
                this.currentHP = (float)stream.ReceiveNext();
            }
        }
    }
}
