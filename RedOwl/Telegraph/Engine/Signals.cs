using System;

namespace RedOwl.Engine
{
    internal interface ISignal {}
    
    internal class Signal : ISignal
    {
        private Action _callback;
        public void Subscribe(Action handler) => _callback += handler;
        public void Unsubscribe(Action handler) => _callback -= handler;
        public void Send() => _callback?.Invoke();
    }

    internal class Signal<T1> : ISignal
    {
        private Action<T1> _callback;
        public void Subscribe(Action<T1> handler) => _callback += handler;
        public void Unsubscribe(Action<T1> handler) => _callback -= handler;
        public void Send(T1 arg1) => _callback?.Invoke(arg1);
    }

    internal class Signal<T1, T2> : ISignal
    {
        private Action<T1, T2> _callback;
        public void Subscribe(Action<T1, T2> handler) => _callback += handler;
        public void Unsubscribe(Action<T1, T2> handler) => _callback -= handler;
        public void Send(T1 arg1, T2 arg2) => _callback?.Invoke(arg1, arg2);
    }

    internal class Signal<T1, T2, T3> : ISignal
    {
        private Action<T1, T2, T3> _callback;
        public void Subscribe(Action<T1, T2, T3> handler) => _callback += handler;
        public void Unsubscribe(Action<T1, T2, T3> handler) => _callback -= handler;
        public void Send(T1 arg1, T2 arg2, T3 arg3) => _callback?.Invoke(arg1, arg2, arg3);
    }

    internal class Signal<T1, T2, T3, T4> : ISignal
    {
        private Action<T1, T2, T3, T4> _callback;
        public void Subscribe(Action<T1, T2, T3, T4> handler) => _callback += handler;
        public void Unsubscribe(Action<T1, T2, T3, T4> handler) => _callback -= handler;
        public void Send(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _callback?.Invoke(arg1, arg2, arg3, arg4);
    }
}