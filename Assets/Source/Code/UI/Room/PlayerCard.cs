using Photon.Realtime;
using Source.Code.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class PlayerCard : MonoBehaviour
    {
        public enum States
        {
            NotLoaded,
            LoadedNotReady,
            Ready
        }

        [SerializeField] private TextMeshProUGUI nickTMP;
        [SerializeField] private Image background;
        [SerializeField] private Image frame;
        [SerializeField] private Image hostCrown;
        [SerializeField] private Color notLoadedColor;
        [SerializeField] private Color loadedNotReadyColor;
        [SerializeField] private Color readyColor;

        private Vector3 cachedPos;

        public Player Owner { get; private set; }
        public int PlaceID { get; set; }
        public Transform Transform { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public States Current { get; private set; }

        public void Initialize(Player owner, int placeID)
        {
            Transform = transform;
            RectTransform = GetComponent<RectTransform>();
            Owner = owner;
            PlaceID = placeID;
            nickTMP.text = owner.NickName;
            cachedPos = RectTransform.position;
        }

        public void SetState(States state)
        {
            if (state == Current) return;
            Color newColor = Color.white;
            switch (state)
            {
                case States.NotLoaded:
                    newColor = notLoadedColor;
                    break;
                case States.LoadedNotReady:
                    newColor = loadedNotReadyColor;
                    break;
                case States.Ready:
                    newColor = readyColor;
                    break;
            }
            background.color = newColor;
        }

        public void SetHostCrownActive(bool isActive)
        {
            hostCrown.enabled = isActive;
        }

        public void SetCardAlpha(float alphaValue)
        {
            background.color = SetAlpha(background.color, alphaValue);
            frame.color = SetAlpha(frame.color, alphaValue);
            hostCrown.color = SetAlpha(hostCrown.color, alphaValue);
            nickTMP.color = SetAlpha(nickTMP.color, alphaValue);
        }

        public void ResetCardPos()
        {
            RectTransform.position = cachedPos;
        }

        public void ChangePlace(int newPlace, Transform newParent)
        {
            Transform.SetParent(newParent);
            RectTransform.SetCornersZero();
            PlaceID = newPlace;
            cachedPos = RectTransform.position;
        }

        private Color SetAlpha(Color color, float alphaValue)
        {
            alphaValue = Mathf.Clamp(alphaValue, 0, 255);
            color.a = alphaValue;
            return color;
        }
    }
}