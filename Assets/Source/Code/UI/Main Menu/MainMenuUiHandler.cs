using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Code.UI.MainMenu
{
    public class MainMenuUiHandler : MonoBehaviour
    {
        public void PlayButton()
        {
            SceneManager.LoadScene(1);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}