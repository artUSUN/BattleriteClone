using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.Extensions;
using Source.Code.MyPhoton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Source.Code.UI.Room
{
    public class RoomCardSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform[] cardPlaces;
        [SerializeField] private GameObject cardPrefab;

        private readonly Dictionary<int, PlayerCard> cards = new Dictionary<int, PlayerCard>(); //playerID, playerCard

        public void Initialize(Player[] players)
        {
            foreach (var player in players)
            {
                if (player.CustomProperties.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object placeId))
                {
                    CreateNewCard(player, (int)placeId);
                }
            }
        }

        public static void SetAvailableCardPlace(Player player)
        {
            var players = PhotonNetwork.PlayerList;
            int availableCardPlace = 0;
            List<int> occupiedCardPlaces = new List<int>();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].ActorNumber == player.ActorNumber) continue;
                if (players[i].CustomProperties.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object playerCardID))
                {
                    occupiedCardPlaces.Add((int)playerCardID);
                }
            }
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                if (occupiedCardPlaces.Contains(i)) continue;
                else
                {
                    availableCardPlace = i;
                    break;
                }
            }
            Hashtable props = new Hashtable { { GlobalConst.PLAYER_CARD_POSITION_ID, availableCardPlace } };
            player.SetCustomProperties(props);
        }

        public void CreateNewCard(Player cardOwner, int playerCardID)
        {
            var cardGO = Instantiate(cardPrefab, cardPlaces[playerCardID]);
            var card = cardGO.GetComponent<PlayerCard>();
            card.Initialize(cardOwner, playerCardID);
            if (cardOwner.ActorNumber != PhotonNetwork.MasterClient.ActorNumber)
            {
                card.SetHostCrownActive(false);
            }
            SetCardState(card, cardOwner);
            cards.Add(cardOwner.ActorNumber, card);
        }

        public void OnPlayerLeave(int playerID)
        {
            Destroy(cards[playerID].gameObject);
            cards.Remove(playerID);
        }

        public void OnCardPositionChanged(Player player, int newPosition)
        {
            if (cards.TryGetValue(player.ActorNumber, out PlayerCard card))
            {
                ChangeCardPlace(card, newPosition);
            }
            else CreateNewCard(player, newPosition);
        }

        public void SetPlayerReady(Player player, bool isReady)
        {
            if (cards.TryGetValue(player.ActorNumber, out PlayerCard card))
            {
                if (isReady) card.SetState(PlayerCard.States.Ready);
                else card.SetState(PlayerCard.States.LoadedNotReady);
            }
        }

        public void OnMasterClientChanged(Player newMaster)
        {
            if (cards.TryGetValue(newMaster.ActorNumber, out PlayerCard card))
            {
                card.SetHostCrownActive(true);
            }
        }

        public void SwapCards(PlayerCard firstCard, PlayerCard secondCard)
        {
            int firstCardNewPlace = secondCard.PlaceID;
            int secondCardNewPlace = firstCard.PlaceID;

            Hashtable props = new Hashtable { { GlobalConst.PLAYER_CARD_POSITION_ID, firstCardNewPlace } };
            firstCard.Owner.SetCustomProperties(props);
            props = new Hashtable { { GlobalConst.PLAYER_CARD_POSITION_ID, secondCardNewPlace } };
            secondCard.Owner.SetCustomProperties(props);

            if (PhotonNetwork.IsMasterClient)
            {
                ChangeCardPlace(firstCard, firstCardNewPlace);
                ChangeCardPlace(secondCard, secondCardNewPlace);
            }
        }

        public bool SetNewCardPlace(PlayerCard card, RectTransform newCardPlace)
        {
            if (cardPlaces.Contains(newCardPlace))
            {
                int newCardPlaceID = Array.IndexOf(cardPlaces, newCardPlace);

                if (card.Owner.CustomProperties.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object currentPos))
                {
                    if ((int)currentPos == newCardPlaceID) return false;
                }

                Hashtable props = new Hashtable { { GlobalConst.PLAYER_CARD_POSITION_ID, newCardPlaceID } };
                card.Owner.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    ChangeCardPlace(card, newCardPlaceID);
                }

                return true;
            }
            else
            {
                Debug.LogError($"Card places doesn't contains {newCardPlace.name}", newCardPlace.transform);
                return false;
            }
        }

        public int GetPlayerFaction(Player player)
        {
            player.CustomProperties.TryGetValue(GlobalConst.PLAYER_CARD_POSITION_ID, out object playerPlaceID);
            return (int)playerPlaceID < 3 ? 0 : 1;
        }

        private void ChangeCardPlace(PlayerCard card, int newPlace)
        {
            card.ChangePlace(newPlace, cardPlaces[newPlace]);
        }

        private void SetCardState(PlayerCard card, Player player)
        {
            bool isLoaded = PhotonExtensions.GetValueOrReturnDefault<bool>(player.CustomProperties, GlobalConst.PLAYER_LOADED_LEVEL);
            bool isReady = PhotonExtensions.GetValueOrReturnDefault<bool>(player.CustomProperties, GlobalConst.PLAYER_READY);

            if (!isLoaded)
            {
                card.SetState(PlayerCard.States.NotLoaded);
            }
            else if (!isReady)
            {
                card.SetState(PlayerCard.States.LoadedNotReady);
            }
            else
            {
                card.SetState(PlayerCard.States.Ready);
            }
        }
    }

}