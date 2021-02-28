using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Source.Code.UI.Room
{
    public class CardDragger : MonoBehaviour
    {
        [SerializeField] private float moveableCardAlphaValue = 150f;
        [Header("Links")]
        [SerializeField] private RoomCardSystem cardSystem;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private EventSystem eventSystem;

        private PlayerCard moveableCard;
        private Vector3 lastMousePos;

        public void Initialize()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) OnClickedDown();
            if (Input.GetKeyUp(KeyCode.Mouse0)) OnClickedUp();

            DragCard();

            lastMousePos = Input.mousePosition;
        }

        private void OnClickedDown()
        {
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("PlayerCard"))
                {
                    var card = result.gameObject.GetComponent<PlayerCard>();
                    if (card != null) StartDraggingCard(card);
                    return;
                }
            }
        }

        private void OnClickedUp()
        {
            if (moveableCard == null) return;

            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            PlayerCard cardToSwap = null;
            RectTransform cardPlace = null;

            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("PlayerCard"))
                {
                    var card = result.gameObject.GetComponent<PlayerCard>();
                    if (card == moveableCard) continue;
                    else cardToSwap = card;
                }

                if (result.gameObject.CompareTag("CardPlace"))
                {
                    cardPlace = result.gameObject.GetComponent<RectTransform>();
                }
            }

            if (cardToSwap != null)
            {
                cardSystem.SwapCards(moveableCard, cardToSwap);              
            }
            else if (cardPlace != null)
            {
                if (cardSystem.SetNewCardPlace(moveableCard, cardPlace) == false) moveableCard.ResetCardPos();
            }
            else
            {
                moveableCard.ResetCardPos();
            }


            moveableCard.SetCardAlpha(255);
            moveableCard = null;
        }

        private void StartDraggingCard(PlayerCard card)
        {
            moveableCard = card;
            moveableCard.SetCardAlpha(moveableCardAlphaValue);
        }

        private void DragCard()
        {
            if (moveableCard == null) return;

            var screenPoint = RectTransformUtility.WorldToScreenPoint(null, moveableCard.RectTransform.position);
            Vector2 screenDelta = Input.mousePosition - lastMousePos;
            screenPoint += screenDelta;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(moveableCard.RectTransform.parent as RectTransform, screenPoint, null, out Vector3 worldPoint) == true)
            {
                moveableCard.RectTransform.position = worldPoint;
            }
        }
    }
}