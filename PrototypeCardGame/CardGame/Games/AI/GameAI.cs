using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrototypeCardGame.Games.AI
{
    /// <summary>
    /// ゲーム AI 共通インターフェース
    /// </summary>
    public abstract class GameAI
    {
        /// <summary>
        /// カード操作タスクリスト
        /// </summary>
        public List<CardManipulateTask> CardManipulateTasks { get; set; } = new List<CardManipulateTask>();

        /// <summary>
        /// カード操作定義
        /// </summary>
        public enum CardManipulation
        {
            /// <summary>
            /// 何も行わない
            /// </summary>
            None,

            /// <summary>
            /// 召喚
            /// </summary>
            Summon,

            /// <summary>
            /// フィールドから召喚コストとして破棄
            /// </summary>
            Sacrifice,

            /// <summary>
            /// 手札、もしくはデッキから直接破棄
            /// </summary>
            Discard
        }

        /// <summary>
        /// カード操作タスク
        /// </summary>
        public class CardManipulateTask
        {
            public CardManipulation Manipulation { get; set; }
            public List<Cards.BattlerCard> Cards { get; set; } = new List<Cards.BattlerCard>();
            public List<int> FieldIndices { get; set; } = new List<int>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player">AI が操作する対象のフィールド</param>
        /// <param name="opponent">対戦相手のフィールド</param>
        public GameAI(DuelPlayer player, DuelPlayer opponent)
        {
            _player = player;
            _opponent = opponent;
        }

        /// <summary>
        /// AI が操作する対象のフィールド
        /// </summary>
        protected DuelPlayer _player;

        /// <summary>
        /// 対戦相手のフィールド
        /// </summary>
        protected DuelPlayer _opponent;
    }

    /// <summary>
    /// シンプルな AI
    /// </summary>
    public class SimpleAI : GameAI
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player"></param>
        /// <param name="opponent"></param>
        public SimpleAI(DuelPlayer player, DuelPlayer opponent) : base(player, opponent)
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CardManipulateTask> Think()
        {
            var cardManipulateTasks = new List<CardManipulateTask>();

            var target = _player.Field.Hands.GetHighestAttackCard();
            
            if (target != null)
            {
                int cost = target.CurrentStatus.Cost;

                if (target.CurrentStatus.Cost > 0)
                {
                    var sacrificeTask = new CardManipulateTask();
                    sacrificeTask.Manipulation = CardManipulation.Sacrifice;
                    foreach (var areaCard in _player.Field.CardAreas)
                    {
                        if (target.CurrentStatus.Attack > areaCard.CurrentStatus.Attack)
                        {
                            sacrificeTask.Cards.Add(areaCard);
                            if (--cost >= 0)
                            {
                                cardManipulateTasks.Add(sacrificeTask);
                                break;
                            }
                        }
                    }
                }

                if (cost == 0)
                {
                    cardManipulateTasks.Add(new CardManipulateTask()
                    {
                        Manipulation = CardManipulation.Summon,
                        Cards = new List<Cards.BattlerCard>() { target }
                    });
                }
            }

            return cardManipulateTasks;
        }
    }
}
