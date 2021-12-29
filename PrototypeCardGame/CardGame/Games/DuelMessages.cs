using System;
using System.Collections.Generic;
using System.Text;
using PrototypeCardGame.Cards;

namespace PrototypeCardGame.Games
{
    /// <summary>
    /// デュエルに関するメッセージの基底クラス
    /// </summary>
    public abstract class DuelMessage : Systems.Message
    {
        /// <summary>
        /// 現在の攻撃手
        /// </summary>
        public PlayField Offenser { get; set; }
        
        /// <summary>
        /// 現在の守備手
        /// </summary>
        public PlayField Defenser { get; set; }
    }

    /// <summary>
    /// フェーズが更新された時のメッセージ
    /// </summary>
    public class DuelUpdatePhaseMessage : DuelMessage
    {
        public Phase Phase { get; }

        public DuelUpdatePhaseMessage(Phase phase) => Phase = phase;
    }

    /// <summary>
    /// カードがドローされた時のメッセージ
    /// </summary>
    public class DuelDrawCardMessage : DuelMessage
    {
        public Card Card { get; set; }

        public bool CanNot { get; set; }
    }

    /// <summary>
    /// プレイヤーにダメージが発生したときのメッセージ
    /// </summary>
    public class DuelDamageMessage : DuelMessage
    {
        public Card Card { get; set; }
        public int Damage { get; set; }
    }

    /// <summary>
    /// カードにダメージが発生したときのメッセージ
    /// </summary>
    public class DuelCardDamageMessage : DuelMessage
    {
        public int Damage { get; set; }

        public Card Card { get; set; }
        public Card DamagedCard { get; set; }

        public int AreaIndex { get; set; }
    }

    /// <summary>
    /// カードがセットされた時のメッセージ
    /// </summary>
    public class DuelSetCardInAreaMessage : DuelMessage
    {
        public Card Card { get; set; }

        public int AreaIndex { get; set; }
    }

    /// <summary>
    /// カードが倒された時のメッセージ
    /// </summary>
    public class DuelDeadCardMessage : DuelMessage
    {
        public Card Card { get; set; }
        public Card DeadCard { get; set; }

        public int AreaIndex { get; set; }
    }

    /// <summary>
    /// デュエルに決着がついた時のメッセージ
    /// </summary>
    public class DuelMatchEndMessage : DuelMessage
    {
        public PlayField Winner { get; set; }
        public PlayField Looser { get; set; }
    }
}
