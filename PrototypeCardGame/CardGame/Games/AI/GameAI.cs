using System;
using System.Collections.Generic;
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
            public List<int> HandIndices { get; set; } = new List<int>();
            public List<int> FieldIndices { get; set; } = new List<int>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="player">AI が操作する対象のフィールド</param>
        /// <param name="opponent">対戦相手のフィールド</param>
        public GameAI(PlayField player, PlayField opponent)
        {
            playField = player;
            opponentField = opponent;
        }

        /// <summary>
        /// AI が操作する対象のフィールド
        /// </summary>
        protected PlayField playField { get; }

        /// <summary>
        /// 対戦相手のフィールド
        /// </summary>
        protected PlayField opponentField { get; }
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
    }
}
