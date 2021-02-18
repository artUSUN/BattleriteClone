using Source.Code.Units;
using Source.Code.Utils;
using UnityEngine;

namespace Source.Code.PlayerInput
{
    public class LookPivot : MonoBehaviour
    {
        public Transform Transform { get; private set; }

        #region Сonstructor
        public static LookPivot New(Transform parent)
        {
            var newGO = Instantiate(GlobalSettingsLoader.Load().Input.LookPivotPrefab, parent);
            var lookPivot = newGO.GetComponent<LookPivot>();
            lookPivot.Initialize();
            return lookPivot;
        }

        private void Initialize()
        {
            Transform = transform;
        }
        #endregion

        public void SetPosition(Vector3 position)
        {
            if (Transform.position == position) return;

            Transform.position = position;
        }
    }
}