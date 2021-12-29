using System;
using System.Collections.Generic;
using System.Text;
using PrototypeCardGame.Cards;

namespace PrototypeCardGame.Games
{
    public class PlayFieldDamageMessage : Systems.Message
    {
        public int Life { get; set; }

        public int Damage { get; set; }

        public int BeforeLife { get; set; }
    }

    public class PlayFieldSetCardInAreaMessage : Systems.Message
    {
        public Card Card { get; set; }

        public int AreaIndex { get; set; }
    }

    public class PlayFieldDeadCardMessage : Systems.Message
    {
        public Card Card { get; set; }

        public int AreaIndex { get; set; }
    }

    public class PlayFieldCardEffectActivatedMessage : Systems.Message
    {
        public CardEffect ActivatedEffect { get; set; }
    }


    public class PlayField : Systems.MessageSender
    {
        public static readonly int CardAreaNum = 4;

        public string Name { get; set; }

        public int Life { get; set; }
        public int TurnCount { get; set; }

        public Deck Deck { get; set; }

        public List<BattlerCard> CardAreas { get; set; } = new List<BattlerCard>(new BattlerCard[CardAreaNum]);

        public List<BattlerCard> Hands { get; set; } = new List<BattlerCard>();

        public List<BattlerCard> Cemetery { get; set; } = new List<BattlerCard>();

        public PlayField(Deck deck) => Deck = deck;

        public bool IsDrawFromDeck { get { return Deck.Cards.Count > 0; } }

        public Card DrawFromDeck()
        {
            if (IsDrawFromDeck)
            {
                var card = Deck.Cards[0];
                Hands.Add(card);
                Deck.Cards.Remove(card);
                return card;
            }
            return null;
        }

        public bool SetCardInArea(int handIndex, int areaIndex)
        {
            if (handIndex < 0 || handIndex >= Hands.Count) return false;
            if (areaIndex < 0 || areaIndex >= CardAreas.Count) return false;
            if (CardAreas[areaIndex] != null) return false;

            var card = Hands[handIndex];
            CardAreas[areaIndex] = card;
            Hands.Remove(card);

            SendMessage(new PlayFieldSetCardInAreaMessage()
            {
                Card = card,
                AreaIndex = areaIndex
            });

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

        public void Damage(int damage, List<int> areas)
        {
            int beforeLife = Life;

            foreach (var index in areas)
            {
                if (index < 0 || index >= CardAreas.Count) continue;

                var card = CardAreas[index];
                
                if (card == null)
                {
                    Life = Life - damage;

                    SendMessage(new PlayFieldDamageMessage()
                    {
                        Life = Life,
                        Damage = damage,
                        BeforeLife = beforeLife,
                    });
                }
                else
                {
                    if (card.CurrentStatus.Life <= damage)
                    {
                        SendMessage(new PlayFieldDeadCardMessage()
                        {
                            Card = card,
                            AreaIndex = index
                        });
                        Cemetery.Add(card);
                        CardAreas.Remove(card);
                    }
                }
            }
        }
    }
}
