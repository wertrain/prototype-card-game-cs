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
            None,
            Summon,
        }

        /// <summary>
        /// カード操作タスク
        /// </summary>
        public class CardManipulateTask
        {
            public CardManipulation Manipulation { get; set; }
            public List<Cards.BattlerCard> Hands { get; set; } = new List<Cards.BattlerCard>();
            public List<int> FieldIndices { get; set; } = new List<int>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player">AI が操作する対象のフィールド</param>
        /// <param name="opponent">対戦相手のフィールド</param>
        public GameAI(PlayField player, PlayField opponent)
        {
            _playField = player;
            _opponentField = opponent;
        }

        /// <summary>
        /// AI が操作する対象のフィールド
        /// </summary>
        protected PlayField _playField;

        /// <summary>
        /// 対戦相手のフィールド
        /// </summary>
        protected PlayField _opponentField;
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
        public SimpleAI(PlayField player, PlayField opponent) : base(player, opponent)
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CardManipulateTask> Think()
        {
            var cardManipulateTasks = new List<CardManipulateTask>();

            var target = _playField.Hands.GetHighestAttackCard();
            if (target != null)
            {
                if (target.CurrentStatus.Cost <= 0)
                {
                    var task = new CardManipulateTask()
                    {
                        Manipulation = CardManipulation.Summon,
                    };
                    task.Hands.Add(target);
                    cardManipulateTasks.Add(task);
                }

                foreach (var areaCard in _playField.CardAreas)
                {
                    if (target.CurrentStatus.Attack > areaCard.CurrentStatus.Attack)
                    {

                    }
                }
            }

            return cardManipulateTasks;
        }
    }
}
