using System;

namespace PrototypeCardGame
{
    class Program
    {
        class Announcer : Systems.IMessageObserver
        {
            public void OnProcessMessage(Systems.Message message)
            {
                switch (message)
                {
                    case Games.DuelUpdatePhaseMessage duelMessage:
                        Console.WriteLine($"[{duelMessage.Phase} フェーズの開始]");
                        if (duelMessage.Phase == Games.Phase.Change)
                        {
                            Console.WriteLine($"---- {duelMessage.Offenser.Name} のターン（{duelMessage.Offenser.TurnCount + 1}） ----");
                        }
                        break;
                    case Games.DuelDrawCardMessage duelMessage:
                        if (!duelMessage.CanNot)
                        {
                            Console.WriteLine($"{duelMessage.Offenser.Name} はカード {duelMessage.Card.GetType().Name} をドロー！");
                        }
                        else
                        {
                            Console.WriteLine($"{duelMessage.Offenser.Name} はカードのドローに失敗...");
                        }
                        break;
                    case Games.DuelDamageMessage duelMessage:
                        Console.Write($"{duelMessage.Offenser.Name} のカード {duelMessage.Card.GetType().Name} の攻撃により ");
                        Console.WriteLine($"{duelMessage.Defenser.Name} は {duelMessage.Damage} ダメージ！");
                        break;
                    case Games.DuelDeadCardMessage duelMessage:
                        Console.Write($"{duelMessage.Offenser.Name} のカード {duelMessage.Card.GetType().Name} の攻撃により ");
                        Console.WriteLine($"{duelMessage.Defenser.Name} のカード {duelMessage.DeadCard.GetType().Name} は墓地に送られた...");
                        break;
                    case Games.DuelCardDamageMessage duelMessage:
                        Console.Write($"{duelMessage.Offenser.Name} のカード {duelMessage.Card.GetType().Name} の攻撃により ");
                        Console.WriteLine($"{duelMessage.Defenser.Name} のカード {duelMessage.DamagedCard.GetType().Name} は {duelMessage.Damage} ダメージ！");
                        break;
                    case Games.DuelSacrificeCardInAreaMessage duelMessage:
                        Console.Write($"{duelMessage.Offenser.Name} はカード "); 
                        foreach (var card in duelMessage.Cards)
                        {
                            Console.Write($"{card.GetType().Name} ");
                        }

                        Console.WriteLine($"を生贄に捧げ...");
                        break;
                    case Games.DuelSetCardInAreaMessage duelMessage:
                        Console.WriteLine($"{duelMessage.Offenser.Name} は {duelMessage.Card.GetType().Name} を召喚！");
                        break;
                    case Games.DuelMatchEndMessage duelMessage:
                        Console.WriteLine($"{duelMessage.Winner.Name} の勝利！");
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            var announcer = new Announcer();
            var duel = new Games.DuelManager();
            duel.SetupSample();
            duel.AddReceiver(announcer);
            
            while (true)
            {
                //Console.ReadLine();
                duel.Step();
            }
        }
    }
}
