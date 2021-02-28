using UnityEngine;

namespace Source.Code.UI.Room
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private SettingsField[] fields;

        public void Initialize(bool isMaster)
        {
            InitializeFields(isMaster);
        }

        public void SetFieldsInteractable(bool isInter)
        {
            foreach (var field in fields)
            {
                field.InputField.interactable = isInter;
            }
        }

        private void InitializeFields(bool isMaster)
        {
            foreach (var field in fields)
            {
                field.InputField.interactable = isMaster;
                field.Initialize(isMaster);
            }
        }
    }
}