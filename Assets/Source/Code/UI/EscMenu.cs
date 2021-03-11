using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
    }
}
