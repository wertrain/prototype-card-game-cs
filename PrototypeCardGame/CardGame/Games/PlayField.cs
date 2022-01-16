using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrototypeCardGame.Cards;

namespace PrototypeCardGame.Games
{
    public class PlayField : Systems.MessageSender
    {
        public static readonly int CardAreaNum = 4;

        public Deck Deck { get; set; }

        public CardSet CardAreas { get; private set; } = new CardSet(CardAreaNum);

        public CardSet Hands { get; private set; } = new CardSet();

        public CardSet Cemetery { get; private set; } = new CardSet();


        public PlayField(Deck deck) => Deck = deck;

        public Card DrawFromDeck()
        {
            var card = Deck.Draw();
            if (card == null) return null;

            Hands.Add((BattlerCard)card);
            return card;

        }

        public bool SetCardInArea(int handIndex, int areaIndex)
        {
            if (handIndex < 0 || handIndex >= Hands.Count) return false;
            if (areaIndex < 0 || areaIndex >= CardAreas.Count) return false;
            if (CardAreas[areaIndex] != null) return false;

            var card = Hands[handIndex];
            CardAreas[areaIndex] = card;
            Hands.Remove(card);

            return true;
        }

        public bool SetCardInArea(BattlerCard card, int areaIndex)
        {
            if (areaIndex < 0 || areaIndex >= CardAreas.Count) return false;
            if (CardAreas[areaIndex] != null) return false;

            if (!Hands.Remove(card)) return false;

            CardAreas[areaIndex] = card;

            return true;
        }

        public void AreaCardToCemetery(int areaIndex)
        {
            if (areaIndex < 0 || areaIndex >= CardAreas.Count) return;

            var card = CardAreas[areaIndex];
            if (card == null) return;

            Cemetery.Add(card);
            CardAreas[areaIndex] = null;
        }

        public void AreaCardToCemetery(BattlerCard card)
        {
            for (int index = 0; index < CardAreas.Count; ++index)
            {
                if (CardAreas[index] == card)
                {
                    Cemetery.Add(card);
                    CardAreas[index] = null;
                    break;
                }
            }
        }
    }
}
