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

        public string Name { get; set; }

        public int Life { get; set; }
        public int TurnCount { get; set; }

        public Deck Deck { get; set; }

        public CardSet CardAreas { get; private set; } = new CardSet(CardAreaNum);

        public CardSet Hands { get; private set; } = new CardSet();

        public CardSet Cemetery { get; private set; } = new CardSet();


        public PlayField(Deck deck) => Deck = deck;

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
    }
}
