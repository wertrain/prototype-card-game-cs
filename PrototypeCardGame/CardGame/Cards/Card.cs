using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrototypeCardGame.Cards
{
    /// <summary>
    /// カードの基底インターフェース
    /// </summary>
    interface ICard
    {

    }

    /// <summary>
    /// カードの基底クラス
    /// </summary>
    public abstract class Card : ICard
    {

    }

    /// <summary>
    /// アイテムカードクラス
    /// </summary>
    public class ItemCard : Card
    {

    }

    /// <summary>
    /// 戦闘を行うカードクラス
    /// </summary>
    public abstract class BattlerCard : Card
    {
        /// <summary>
        /// 初期ステータスを定義
        /// </summary>
        /// <returns></returns>
        public abstract Status GetDefaultStatus();

        /// <summary>
        /// ステータス
        /// </summary>
        public class Status
        {
            public int Cost { get; set; }
            public int Life { get; set; }
            public int Attack { get; set; }
        }

        /// <summary>
        /// 現在のステータスを取得
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        /// 初期ステータスを取得
        /// </summary>
        public Status DefaultStatus { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattlerCard() => CurrentStatus = GetDefaultStatus();
    }

    /// <summary>
    /// 一組のカードクラス
    /// </summary>
    public abstract class CardSetBase<CardType> : IEnumerable<CardType> where CardType : Card
    {
        /// <summary>
        /// カード枚数
        /// </summary>
        public int FixedCount { get; private set; }

        /// <summary>
        /// カード配列
        /// </summary>
        public List<CardType> Cards { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CardSetBase() => Cards = new List<CardType>();

        /// <summary>
        /// 初期カード数を指定したコンストラクタ
        /// </summary>
        /// <param name="num"></param>
        public CardSetBase(int num)
        {
            FixedCount = num;
            Cards = new List<CardType>(new CardType[num]);
        }

        /// <summary>
        /// カード枚数
        /// </summary>
        public int Count { get { return Cards.Count; } }

        /// <summary>
        /// 列挙型を取得
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CardType> GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        /// <summary>
        /// 列挙型を取得
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        /// <summary>
        /// カードを追加
        /// </summary>
        /// <param name="card"></param>
        public bool Add(CardType card)
        {
            if (FixedCount == 0)
            {
                Cards.Add(card);
                return true;
            }
            else
            {
                for (int index = 0; index < FixedCount; ++index)
                {
                    if (Cards[index] == null)
                    {
                        Cards[index] = card;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// カードを削除
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool Remove(CardType card)
        {
            if (FixedCount == 0)
            {
                return Cards.Remove(card);
            }
            else
            {
                for (int index = 0; index < FixedCount; ++index)
                {
                    if (Cards[index] == card)
                    {
                        Cards[index] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public List<int> GetEmptyIndices()
        {
            if (FixedCount == 0)
            {
                return null;
            }
            else
            {
                var indices = new List<int>();
                for (int index = 0; index < FixedCount; ++index)
                {
                    if (Cards[index] == null)
                    {
                        indices.Add(index);
                    }
                }
                return indices;
            }
        }

        /// <summary>
        /// インデクサー
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CardType this[int index]
        {
            set { Cards[index] = value; }
            get { return Cards[index]; }
        }

        /// <summary>
        /// カードをシャッフル
        /// </summary>
        public void Shuffle()
        {
            var rnd = new Random();
            Cards = Cards.OrderBy(item => rnd.Next()).ToList();
        }
    }
    
    /// <summary>
    /// バトル時のカード
    /// </summary>
    public class CardSet : CardSetBase<BattlerCard>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CardSet() : base() { }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num"></param>
        public CardSet(int num) : base(num) { }

        /// <summary>
        /// 最も攻撃力の高いカードを取得
        /// </summary>
        /// <returns></returns>
        public BattlerCard GetHighestAttackCard()
        {
            return GetHighestAttackCards().FirstOrDefault();
        }

        /// <summary>
        /// 最も攻撃力の高いカードを取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BattlerCard> GetHighestAttackCards()
        {
            return Cards.OrderBy(x => x.CurrentStatus.Attack);
        }

        /// <summary>
        /// 最もライフの高いカードを取得
        /// </summary>
        /// <returns></returns>
        public BattlerCard GetHighestLifeCard()
        {
            return GetHighestLifeCards().FirstOrDefault();
        }

        /// <summary>
        /// 最もライフの高いカードを取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BattlerCard> GetHighestLifeCards()
        {
            return Cards.OrderBy(x => x.CurrentStatus.Life);
        }

        /// <summary>
        /// 最もコストの高いカードを取得
        /// </summary>
        /// <returns></returns>
        public BattlerCard GetHighestCostCard()
        {
            return GetHighestLifeCards().FirstOrDefault();
        }
    }

    /// <summary>
    /// デッキクラス
    /// </summary>
    public class Deck : CardSetBase<Card>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Deck() : base() { }

        /// <summary>
        /// カードをドローできるかを取得
        /// </summary>
        public bool IsDraw { get { return Cards.Count > 0; } }

        /// <summary>
        /// カードをドロー
        /// </summary>
        /// <returns>ドローしたカード</returns>
        public Card Draw()
        {
            var card = Cards.FirstOrDefault();
            if (card == null) return null;
            Cards.Remove(card);
            return card;
        }
    }

    /// <summary>
    /// カード効果の定義
    /// </summary>
    public abstract class CardEffect
    {
        /// <summary>
        /// 効果発動フェーズ
        /// </summary>
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

        /// <summary>
        /// 効果発動タイプ
        /// </summary>
        enum ActivatedType
        {
            Once,
            Always,
            Count,
        }

        /// <summary>
        /// 効果発動場所
        /// </summary>
        enum ActivatedPlace
        {
            InHand,
            OnField,
            InDeck
        }

        /// <summary>
        /// 効果回数
        /// </summary>
        public int ActivatedCount { get; set; }
    }
}
