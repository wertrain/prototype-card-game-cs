using System;
using System.Collections.Generic;
using System.Text;

namespace PrototypeCardGame.Systems
{
#if false
    public interface IObserver<in T>
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(T value);
    }
#else
    public interface IObserver<in T>
    {

    }
#endif
    public interface IObservable<out T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
}
