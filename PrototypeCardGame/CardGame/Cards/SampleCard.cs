using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrototypeCardGame.Cards
{
    /// <summary>
    /// 「ドラゴン」カード
    /// </summary>
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

    /// <summary>
    /// 「スライム」カード
    /// </summary>
    public class CardSlime : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 0,
                Life = 1,
                Attack = 0,
            };
        }
    }

    /// <summary>
    /// 「戦士」カード
    /// </summary>
    public class CardWarrior : BattlerCard
    {
        public override Status GetDefaultStatus()
        {
            return new Status
            {
                Cost = 1,
                Life = 2,
                Attack = 3,
            };
        }
    }

    /// <summary>
    /// 「魔法使い」カード
    /// </summary>
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
}
