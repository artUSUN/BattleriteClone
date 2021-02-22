using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Source.Code.MyPhoton
{
    public class NickNameSetter : MonoBehaviour
    {
        [SerializeField] private float blinkPeriod = 0.2f;
        [SerializeField] private int blinkCounts = 7;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI placeholderTMP;
        [SerializeField] private int maxSymbols = 15;

        private string currentNick = "";

        private const string ppNickNameKey = "PlayerNickName";

        private void Awake()
        {
            if (PlayerPrefs.HasKey(ppNickNameKey))
            {
                currentNick = PlayerPrefs.GetString(ppNickNameKey);
                inputField.text = currentNick;
                PhotonNetwork.NickName = currentNick;
            }
            
        }

        public void OnInputFieldChanging(string textInInputField)
        {
            if (textInInputField.Length > maxSymbols)
            {
                inputField.text = currentNick;
            }
            else
            {
                currentNick = textInInputField;
            }
        }

        public void OnInputFieldChangingEnded(string textInInputField)
        {
            PhotonNetwork.NickName = currentNick;
            PlayerPrefs.SetString(ppNickNameKey, currentNick);
        }

        public bool IsNicknameEmpty()
        {
            bool isEmpty = currentNick == "";
            if (isEmpty)
            {
                StartCoroutine(Blink(placeholderTMP, blinkPeriod, blinkCounts));
            }
            return isEmpty;
        }

        private IEnumerator Blink(TextMeshProUGUI tmp, float period, int count)
        {
            var waiter = new WaitForSeconds(period);
            for (int i = 0; i < count; i++)
            {
                tmp.color = i % 2 == 0 ? Color.red : Color.black;
                yield return waiter;
            }
        }
    }
}