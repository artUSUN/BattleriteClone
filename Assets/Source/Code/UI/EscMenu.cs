using Photon.Pun;
using UnityEngine;

namespace Source.Code.UI
{
    public class EscMenu : MonoBehaviour
    {
        public void Resume()
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        public void ExitToMainMenu()
        {
            PhotonNetwork.Disconnect();
        }
    }
}
