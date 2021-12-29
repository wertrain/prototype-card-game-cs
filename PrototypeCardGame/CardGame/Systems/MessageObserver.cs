using System;
using System.Collections.Generic;
using System.Text;

namespace PrototypeCardGame.Systems
{
    /// <summary>
    /// メッセージ基底クラス
    /// </summary>
#if false
    public abstract class Message
    {
        public abstract string Name { get; }
    }
#else
    public abstract class Message {}
#endif

    /// <summary>
    /// メッセージ受信者
    /// </summary>
    public interface IMessageObserver : IObserver<Message>
    {
        void OnProcessMessage(Message message);
    }

    /// <summary>
    /// メッセージ送信可能クラス
    /// </summary>
    public abstract class MessageObservable : IObservable<Message>
    {
        /// <summary>
        /// メッセージを送信する
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(Message message)
        {
            foreach (var wrapper in _observers)
            {
                if (wrapper.InterestedClasses.Count == 0 || 
                    wrapper.InterestedClasses.Find(type => type == message.GetType()) != null)
                {
                    wrapper.Observer.OnProcessMessage(message);
                }
            }
        }

        /// <summary>
        /// 購読する
        /// </summary>
        /// <param name="observer"></param>
        /// <returns>購読解除オブジェクト</returns>
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            var observerWrapper = _observers.Find(o => { return o.Observer == observer; });
            if (observerWrapper == null)
            {
                observerWrapper = new ObserverWrapper();
                observerWrapper.Observer = (IMessageObserver)observer;
                _observers.Add(observerWrapper);
            }
            observerWrapper.InterestedClasses.Clear();

            return new Unsubscriber(_observers, observerWrapper);
        }

        /// <summary>
        /// 購読する
        /// </summary>
        /// <typeparam name="MessageType">購読したいメッセージ型</typeparam>
        /// <param name="observer"></param>
        /// <returns>購読解除オブジェクト</returns>
        public IDisposable Subscribe<MessageType>(IObserver<Message> observer) where MessageType : Message
        {
            var unsubscriber = Subscribe(observer);
            var observerWrapper = _observers.Find(o => { return o.Observer == observer; });
            observerWrapper.InterestedClasses.Add(typeof(MessageType));
            return unsubscriber;
        }

        /// <summary>
        /// 購読解除オブジェクト
        /// </summary>
        private class Unsubscriber : IDisposable
        {
            public Unsubscriber(List<ObserverWrapper> observers, ObserverWrapper observer)
            {
                _observers = observers;
                _observer = observer;
            }
            public void Dispose()
            {
                _observers.Remove(_observer);
            }
            private List<ObserverWrapper> _observers;
            private ObserverWrapper _observer;
        }

        /// <summary>
        /// オブザーバーを登録する（Subscribeの別名メソッド）
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public void AddObserver(IObserver<Message> observer) => Subscribe(observer);

        /// <summary>
        /// オブザーバーを登録する（Subscribeの別名メソッド）
        /// </summary>
        /// <typeparam name="MessageType">取得したいメッセージ型</typeparam>
        /// <param name="observer"></param>
        public void AddObserver<MessageType>(IObserver<Message> observer) where MessageType : Message => Subscribe<MessageType>(observer);

        /// <summary>
        /// オブザーバーを削除する（Unsubscriber を使用しない場合に呼び出す）
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(IObserver<Message> observer)
        {
             _observers.RemoveAll(o => o.Observer == observer);
        }

        public bool InterestTo<MessageType>(IMessageObserver observer) where MessageType : Message
        {
            var observerWrapper = _observers.Find(o => { return o.Observer == observer; });
            if (observerWrapper == null) return false;

            return observerWrapper.InterestedClasses.Find(type => type == typeof(MessageType)) != null;
        }

        /// <summary>
        /// オブザーバーのラッパー
        /// </summary>
        private class ObserverWrapper
        {
            public List<Type> InterestedClasses = new List<Type>();
            public IMessageObserver Observer { get; set; }
        }

        private List<ObserverWrapper> _observers = new List<ObserverWrapper>();
    }

    /// <summary>
    /// メッセージの送信者クラス
    /// MessageObservable の別名クラス
    /// </summary>
    public abstract class MessageSender : MessageObservable
    {
        [System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public new IDisposable Subscribe(IObserver<Message> observer) => base.Subscribe(observer);
        [System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public new IDisposable Subscribe<MessageType>(IObserver<Message> observer) where MessageType : Message => base.Subscribe<MessageType>(observer);
        [System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
        public new void RemoveObserver(IObserver<Message> observer) => base.RemoveObserver(observer);

        public void AddReceiver(IObserver<Message> observer) => Subscribe(observer);
        public void AddReceiver<MessageType>(IObserver<Message> observer) where MessageType : Message => Subscribe<MessageType>(observer);
        public void RemoveReceiver(IObserver<Message> observer) => RemoveObserver(observer);
    }

    /// <summary>
    /// メッセージ送受信可能クラス
    /// </summary>
    public abstract class Messenger : MessageSender, IObservable<Message> { }
}
