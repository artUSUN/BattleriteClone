using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Source.Code.MyPhoton;
using System.Collections.Generic;
using UnityEngine;

namespace Source.Code.UI.Room
{
    public class RoomCardSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform[] cardPlaces;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardRoot;

        private readonly Dictionary<int, PlayerCard> cards = new Dictionary<int, PlayerCard>(); //playerID, playerCard

        public RectTransform[] CardPlaces => cardPlaces;

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
            cards.Add(cardOwner.ActorNumber, card);
        }

        public void OnPlayerLeaved(int playerID)
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

        public void OnPlayerLoadedGame(Player player)
        {
            if (cards.TryGetValue(player.ActorNumber, out PlayerCard card))
            {
                card.SetState(PlayerCard.States.LoadedNotReady);
            }
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

        private void ChangeCardPlace(PlayerCard card, int newPlace)
        {
            card.Transform.SetParent(cardPlaces[newPlace]);
            card.PlaceID = newPlace;
        }
    }
}