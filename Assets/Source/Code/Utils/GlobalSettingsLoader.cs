using UnityEngine;

namespace Source.Code.Utils
{
    public static class GlobalSettingsLoader
    {
        private static GlobalSettings currentSettings;

        public static GlobalSettings Load()
        {
            if (!currentSettings) currentSettings = MonoBehaviour.Instantiate(Resources.Load<GlobalSettings>("GlobalSettings/Default"));
            return currentSettings;
        }
    }
}