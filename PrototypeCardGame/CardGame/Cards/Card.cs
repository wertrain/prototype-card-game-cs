using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrototypeCardGame.Cards
{
    interface ICard
    {
    }

    public class Card : ICard
    {
    }

    public class ItemCard : Card
    {
    }

    public abstract class BattlerCard : Card
    {
        public abstract Status GetDefaultStatus();

        public class Status
        {
            public int Cost { get; set; }
            public int Life { get; set; }
            public int Attack { get; set; }
        }

        public Status CurrentStatus { get; set; }

        public Status DefaultStatus { get; set; }

        public BattlerCard()
        {
            CurrentStatus = GetDefaultStatus();
        }
    }

    public class CardDragon : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 3,
                Life = 5,
                Attack = 5,
            };
        }
    }

    public class CardSlime : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 0,
                Life = 1,
                Attack = 1,
            };
        }
    }

    public class CardWarrior : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 1,
                Life = 2,
                Attack = 1,
            };
        }
    }

    public class CardWizard : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 1,
                Life = 1,
                Attack = 2,
            };
        }
    }

    public class Deck
    {
        public List<BattlerCard> Cards { get; set; } = new List<BattlerCard>();

        public void Shuffle()
        {
            var rnd = new Random();
            Cards = Cards.OrderBy(item => rnd.Next()).ToList();
        }
    }

    public abstract class CardEffect
    {
        enum ActivatedPhase
        {
            GameStart,
            BeforeDraw,
            AfterDraw,
            BeforeStandby,
            AfterStandby,
            BeforeCardSet,
            AfterCardSet,
            BeforeAttacked,
            AfterAttacked,
            BeforeAttacking,
            AfterAttacking,
            BeforeDamaged,
            AfterDamaged,
            BeforeDamaging,
            AfterDamaging,
            GameEnd,
        }

        enum ActivatedType
        {
            Once,
            Always,
            Count,
        }

        enum ActivatedPlace
        {
            InHand,
            OnField,
            InDeck
        }

        public int ActivatedCount { get; set; }
    }
}
