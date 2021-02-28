using ExitGames.Client.Photon;
using Photon.Pun;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Source.Code.UI.Room
{
    public class SettingsField : MonoBehaviourPunCallbacks
    {
        public enum Type
        {
            Float,
            Int
        }

        [SerializeField] private Vector2 borderValues;
        [SerializeField] private Type type;
        [SerializeField] private string key;
        [SerializeField] private TMP_InputField inputField;

        private float lastValue;

        public float Value { get; private set; }
        public string Key => key;
        public TMP_InputField InputField => inputField;

        public void Initialize(bool isMaster)
        {
            if (type == Type.Int)
            {
                borderValues.x = Mathf.Round(borderValues.x);
                borderValues.y = Mathf.Round(borderValues.y);
            }

            if (isMaster)
            {
                lastValue = ClampBorders(float.Parse(inputField.text, CultureInfo.InvariantCulture));
                Value = lastValue;

                Hashtable props = new Hashtable { { key, Value } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            else
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object newValue))
                {
                    inputField.text = ((float)newValue).ToString();
                }
                else
                {
                    lastValue = ClampBorders(float.Parse(inputField.text, CultureInfo.InvariantCulture));
                    Value = lastValue;
                }
            }
            
        }

        public void OnEndEdit(string newValue)
        {
            if (type == Type.Int)
            {
                if (int.TryParse(newValue, out int result))
                {
                    Value = ClampBorders(result);
                }
            }
            else if (type == Type.Float)
            {
                if (float.TryParse(newValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float result))
                {
                    Value = ClampBorders(result);
                }
            }

            if (Value != lastValue)
            {
                lastValue = Value;
                Hashtable props = new Hashtable { { key, Value } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }

            inputField.text = lastValue.ToString();
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (PhotonNetwork.IsMasterClient == false)
            {
                if (propertiesThatChanged.TryGetValue(key, out object newValue))
                {
                    inputField.text = ((float)newValue).ToString();
                }
            }
        }

        private float ClampBorders(float valueToClamp)
        {
            return Mathf.Clamp(valueToClamp, borderValues.x, borderValues.y);
        }
    }
}