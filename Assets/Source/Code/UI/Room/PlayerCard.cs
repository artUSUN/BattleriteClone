using Photon.Realtime;
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
        [SerializeField] private Image hostCrown;
        [SerializeField] private Color notLoadedColor;
        [SerializeField] private Color loadedNotReadyColor;
        [SerializeField] private Color readyColor;

        public Player Owner { get; private set; }
        public int PlaceID { get; set; }
        public Transform Transform { get; private set; }
        public States Current { get; private set; }

        public void Initialize(Player owner, int placeID)
        {
            Transform = transform;
            Owner = owner;
            PlaceID = placeID;
            nickTMP.text = owner.NickName;
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
    }
}