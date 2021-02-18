using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.Photon
{
    public class PlayerShooter : MonoBehaviourPun
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletCreationPoint;
        [SerializeField] private float reloadDuration = 2;

        private bool isReloading = false;
        private float timer = 0;
        private Slider slider;

        private void Start()
        {
            if (photonView.IsMine)
            {
                slider = UIHandler.Instance.ReloadSlider;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            if (Input.GetButtonDown("Fire1"))
            {
                if (isReloading == false)
                {
                    Shoot();
                }
            }

            if (isReloading)
            {
                timer += Time.deltaTime;
                slider.value = Mathf.Clamp01(timer / reloadDuration);
                if (timer > reloadDuration) 
                {
                    isReloading = false;
                    timer = 0;
                }
            }
        }

        private void Shoot()
        {
            isReloading = true;
            PhotonNetwork.Instantiate(bulletPrefab.name, bulletCreationPoint.position, bulletCreationPoint.rotation);
        }
    }
}
