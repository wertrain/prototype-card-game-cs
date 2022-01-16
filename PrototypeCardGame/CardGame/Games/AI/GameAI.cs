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
            public List<Cards.Card> Cards { get; set; } = new List<Cards.Card>();
            public List<int> FieldIndices { get; set; } = new List<int>();
            public CardManipulateTask(CardManipulation manipulation) => Manipulation = manipulation;
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

        public abstract List<CardManipulateTask> Think();

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
            CardAttributes = new Dictionary<Cards.Card, Attribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<CardManipulateTask> Think()
        {
            var cardManipulateTasks = new List<CardManipulateTask>();

            var usedFieldIndices = new List<int>();

            var tasks = MakeSummonTasks(usedFieldIndices);
            while (tasks != null)
            {
                cardManipulateTasks.AddRange(tasks);
                tasks = MakeSummonTasks(usedFieldIndices);
            }

            return cardManipulateTasks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<CardManipulateTask> MakeSummonTasks(List<int> usedFieldIndices)
        {
            Cards.BattlerCard targetCard = null;
            
            foreach (var card in _player.Field.Hands.GetHighestAttackCards())
            {
                if (HasAttributes(card, Attribute.Summoned))
                {
                    continue;
                }

                targetCard = card;
                break;
            }

            if (targetCard == null)
            {
                return null;
            }

            var tasks = new List<CardManipulateTask>();

            var summonTask = new CardManipulateTask(CardManipulation.Summon);
            summonTask.Cards.Add(targetCard);

            var indices = new List<int>();

            if (targetCard.CurrentStatus.Cost > 0)
            {
                var sacrificesTask = MakeSacrificesTask(targetCard, 
                    card => card.CurrentStatus.Attack < targetCard.CurrentStatus.Attack
                );

                if (sacrificesTask == null)
                {
                    return null;
                }

                indices.AddRange(sacrificesTask.FieldIndices);
                tasks.Add(sacrificesTask);
            }

            var index = GetSummonIndex(targetCard, indices, usedFieldIndices);
            
            if (index == null)
            {
                return null;
            }

            summonTask.FieldIndices.Add(index.Value);
            usedFieldIndices.Add(index.Value);
            tasks.Add(summonTask);
            AddAttributes(targetCard, Attribute.Summoned);

            return tasks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        private CardManipulateTask MakeSacrificesTask(Cards.BattlerCard target, Func<Cards.BattlerCard, bool> comparison)
        {
            var indices = new List<int>();
            var sacrificeTask = new CardManipulateTask(CardManipulation.Sacrifice);
            var cost = target.CurrentStatus.Cost;

            for (int index = 0; index < _player.Field.CardAreas.Count; ++index)
            {
                var areaCard = _player.Field.CardAreas[index];

                if (areaCard == null)
                {
                    continue;
                }

                if (HasAttributes(areaCard, Attribute.Sacrificed))
                {
                    continue;
                }

                if (comparison(areaCard))
                {
                    indices.Add(index);
                    sacrificeTask.Cards.Add(areaCard);
                    sacrificeTask.FieldIndices = indices;

                    if (--cost <= 0)
                    {
                        foreach (var card in sacrificeTask.Cards)
                        {
                            AddAttributes(card, Attribute.Sacrificed);
                        }
                        return sacrificeTask;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summonCard"></param>
        /// <param name="sacrificeIndices"></param>
        /// <returns></returns>
        private int? GetSummonIndex(Cards.BattlerCard summonCard, List<int> sacrificeIndices, List<int> usedIndices)
        {
            var indices = _player.Field.CardAreas.GetEmptyIndices();
            indices.AddRange(sacrificeIndices);

            for (int index = 0; index < _opponent.Field.CardAreas.Count; ++index)
            {
                if (usedIndices.Contains(index))
                {
                    continue;
                }

                var area = _opponent.Field.CardAreas[index];

                // 敵側にカードがなく、こちらも召喚可能であれば
                if (area == null && indices.Contains(index))
                {
                    return index;
                }
            }

            for (int index = 0; index < _opponent.Field.CardAreas.Count; ++index)
            {
                if (usedIndices.Contains(index))
                {
                    continue;
                }

                var area = _opponent.Field.CardAreas[index];

                // 召喚するカードの方が攻撃力が高ければ
                if (area != null && area.CurrentStatus.Attack < summonCard.CurrentStatus.Attack)
                {
                    return index;
                }
            }

            for (int index = 0; index < _opponent.Field.CardAreas.Count; ++index)
            {
                if (usedIndices.Contains(index))
                {
                    continue;
                }

                var area = _player.Field.CardAreas[index];

                // 空きフィールドであれば
                if (area == null)
                {
                    return index;
                }
            }

            return null;
        }

        /// <summary>
        /// 属性の付与
        /// </summary>
        /// <param name="card"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private bool AddAttributes(Cards.Card card, Attribute attribute)
        {
            if (CardAttributes.ContainsKey(card))
            {
                CardAttributes[card] = CardAttributes[card] | attribute;
            }
            else
            {
                CardAttributes.Add(card, attribute);
            }

            return true;
        }

        /// <summary>
        /// 属性が付与されているか
        /// </summary>
        /// <param name="card"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private bool HasAttributes(Cards.Card card, Attribute attribute)
        {
            if (CardAttributes.ContainsKey(card))
            {
                return CardAttributes[card].HasFlag(attribute);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        enum Attribute
        {
            Sacrificed = 1 << 0,
            Summoned = 1 << 1,
        };

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Cards.Card, Attribute> CardAttributes { get; set; }
    }
}
