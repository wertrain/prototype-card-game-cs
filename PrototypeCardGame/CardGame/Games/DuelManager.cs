using System;
using System.Collections.Generic;
using System.Text;
using PrototypeCardGame.Cards;

namespace PrototypeCardGame.Games
{
    /// <summary>
    /// フェーズの定義
    /// </summary>
    public enum Phase
    {
        Start,
        Draw,
        Standby,
        Battle,
        End,
        Change
    }

    /// <summary>
    /// デュエル管理クラス
    /// </summary>
    public class DuelManager : Systems.MessageSender
    {
        private PlayField _playerField;
        private PlayField _opponentField;

        private List<PlayField> _playingField;

        private Systems.StateMachine<DuelManager> _stateMachine;
        
        public enum PhaseTo : int
        {
            Start,
            Draw,
            Standby,
            Battle,
            End,
            Judge,
            Win,
            Loose,
            Change
        }

        public DuelManager()
        {
            _stateMachine = new Systems.StateMachine<DuelManager>(this);
            _stateMachine.AddTransition<TurnStartPhase, DrawPhase>((int)PhaseTo.Draw);
            _stateMachine.AddTransition<DrawPhase, StandbyPhase>((int)PhaseTo.Standby);
            _stateMachine.AddTransition<StandbyPhase, BattlePhase>((int)PhaseTo.Battle);
            _stateMachine.AddTransition<BattlePhase, TurnEndPhase>((int)PhaseTo.End);
            _stateMachine.AddTransition<TurnEndPhase, JudgePhase>((int)PhaseTo.Judge);
            _stateMachine.AddTransition<JudgePhase, WinPhase>((int)PhaseTo.Win);
            _stateMachine.AddTransition<JudgePhase, LoosePhase>((int)PhaseTo.Loose);
            _stateMachine.AddTransition<JudgePhase, ChangePhase>((int)PhaseTo.Change);
            _stateMachine.AddTransition<ChangePhase, TurnStartPhase>((int)PhaseTo.Start);
            _stateMachine.SetStartState<TurnStartPhase>();
        }

        private PlayField GetCurrentOffenser()
        {
            return _playingField[0];
        }

        private PlayField GetCurrentDefenser()
        {
            return _playingField[1];
        }

        private void ChangeOffenseAndDefense()
        {
            var field = _playingField[0];
            _playingField[0] = _playingField[1];
            _playingField[1] = field;
        }

        public void SendUpdatePhaseMessage(Phase phase)
        {
            SendMessage(new DuelUpdatePhaseMessage(phase)
            {
                Offenser = GetCurrentOffenser(),
                Defenser = GetCurrentDefenser(),
            });
        }

        public void SendDuelMessage<MessageType>(Action<MessageType> construct) where MessageType : DuelMessage, new()
        {
            var message = new MessageType()
            {
                Offenser = GetCurrentOffenser(),
                Defenser = GetCurrentDefenser(),
            };
            construct(message);
            SendMessage(message);
        }

        private class TurnStartPhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendUpdatePhaseMessage(Phase.Start);
                Context._stateMachine.SendEvent((int)PhaseTo.Draw);
            }
        }

        private class DrawPhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendUpdatePhaseMessage(Phase.Draw);
                var card = Context.GetCurrentOffenser().DrawFromDeck();

                Context.SendDuelMessage<DuelDrawCardMessage>(message =>
                {
                    message.Card = card;
                    message.CanNot = card == null;
                });

                Context._stateMachine.SendEvent((int)PhaseTo.Standby);
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class StandbyPhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendUpdatePhaseMessage(Phase.Standby);
                if (Context.GetCurrentOffenser().SetCardInArea(0, 0))
                {
                    Context.SendDuelMessage<DuelSetCardInAreaMessage>(message =>
                    {
                        message.Card = Context.GetCurrentOffenser().CardAreas[0];
                        message.AreaIndex = 0;
                    });
                }
                Context._stateMachine.SendEvent((int)PhaseTo.Battle);
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class BattlePhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendUpdatePhaseMessage(Phase.Battle);

                for (int index = 0; index < Context.GetCurrentOffenser().CardAreas.Count; ++index)
                {
                    var offense = Context.GetCurrentOffenser();
                    var defense = Context.GetCurrentDefenser();
                    var offenseCard = offense.CardAreas[index];
                    var defenseCard = defense.CardAreas[index];

                    if (offenseCard == null) continue;

                    if (defenseCard == null)
                    {
                        int damage = offenseCard.CurrentStatus.Attack;
                        defense.Life = defense.Life - damage;

                        Context.SendDuelMessage<DuelDamageMessage>(message =>
                        {
                            message.Card = offenseCard;
                            message.Damage = damage;
                        });
                    }
                    else
                    {
                        int damage = offenseCard.CurrentStatus.Attack;
                        defenseCard.CurrentStatus.Life = defenseCard.CurrentStatus.Life - damage;
                        
                        if (defenseCard.CurrentStatus.Life <= 0)
                        {
                            defense.AreaCardToCemetery(index);
                            Context.SendDuelMessage<DuelDeadCardMessage>(message =>
                            {
                                message.Card = offenseCard;
                                message.DeadCard = defenseCard;
                                message.AreaIndex = index;
                            });
                        }
                        else
                        {
                            Context.SendDuelMessage<DuelCardDamageMessage>(message =>
                            {
                                message.Damage = damage;
                                message.Card = offenseCard;
                                message.DamagedCard = defenseCard;
                                message.AreaIndex = index;
                            });
                        }
                    }
                }
                Context._stateMachine.SendEvent((int)PhaseTo.End);
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class TurnEndPhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendUpdatePhaseMessage(Phase.End);
                Context._stateMachine.SendEvent((int)PhaseTo.Judge);
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class JudgePhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                if (Context._opponentField.Life <= 0)
                {
                    Context._stateMachine.SendEvent((int)PhaseTo.Win);
                }
                else if (Context._playerField.Life <= 0)
                {
                    Context._stateMachine.SendEvent((int)PhaseTo.Loose);
                }
                else
                {
                    Context._stateMachine.SendEvent((int)PhaseTo.Change);
                }

            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class WinPhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendDuelMessage<DuelMatchEndMessage>(message =>
                {
                    message.Winner = Context._playerField;
                    message.Looser = Context._opponentField;
                });
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class LoosePhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.SendDuelMessage<DuelMatchEndMessage>(message =>
                {
                    message.Winner = Context._opponentField;
                    message.Looser = Context._playerField;
                });
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        private class ChangePhase : Systems.StateMachine<DuelManager>.State
        {
            protected internal override void Enter()
            {
                Context.GetCurrentOffenser().TurnCount++;
                Context.ChangeOffenseAndDefense();
                Context.SendUpdatePhaseMessage(Phase.Change);

                Context._stateMachine.SendEvent((int)PhaseTo.Start);
            }
            protected internal override void Update() { }
            protected internal override void Exit() { }
        }

        public void SetupSample()
        {
            {
                Deck deck = new Deck();
                deck.Cards = new List<BattlerCard>()
                {
                    new CardDragon(),
                    new CardWarrior(),
                    new CardWarrior(),
                    new CardWizard(),
                    new CardSlime()
                };
                deck.Shuffle();
                _playerField = new PlayField(deck);
                _playerField.Life = 10;
                _playerField.Name = "Player";
            }

            {
                Deck deck = new Deck();
                deck.Cards = new List<BattlerCard>()
                {
                    new CardDragon(),
                    new CardSlime(),
                    new CardSlime(),
                    new CardSlime(),
                    new CardSlime()
                };
                deck.Shuffle();
                _opponentField = new PlayField(deck);
                _opponentField.Life = 10;
                _opponentField.Name = "Opponent";
            }

            _playingField = new List<PlayField>();
            _playingField.Add(_playerField);
            _playingField.Add(_opponentField);
        }

        public void Step()
        {
            _stateMachine.Update();
        }
    }
}
