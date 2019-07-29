using System;
using System.Collections.Generic;

namespace RedOwl.Engine
{
    /*
    internal class Signals : Dictionary<Enum, Delegate>
    {
        public void Subscribe(Enum key, TelegraphHandler handler)
        {
            if (!ContainsKey(key)) Add(key, null);
            this[key] = (TelegraphHandler)this[key] + handler;
        }

        public void Subscribe<TMsg>(Enum key, TelegraphHandler<TMsg> handler)
        {
            if (!ContainsKey(key)) Add(key, null);
            this[key] = (TelegraphHandler<TMsg>)this[key] + handler;
        }

        public void Unsubscribe(Enum key, [NotNull] TelegraphHandler handler)
        {
            Delegate callback;
            if (TryGetValue(key, out callback))
            {
                this[key] = (TelegraphHandler)callback - handler;
            }
        }

        public void Unsubscribe<TMsg>(Enum key, TelegraphHandler<TMsg> handler)
        {
            Delegate callback;
            if (TryGetValue(key, out callback))
            {
                this[key] = (TelegraphHandler<TMsg>)callback - handler;
            }
        }
    }
    */
    
    public class EventsManager
    {
        private readonly Dictionary<Enum, ISignal> _signals = new Dictionary<Enum, ISignal>();
        private readonly Dictionary<Enum, ISignal> _signalsOnce = new Dictionary<Enum, ISignal>();
        private readonly Dictionary<Enum, ISignal> _signalsPermanent = new Dictionary<Enum, ISignal>();
        
        private readonly List<Action> _updateQueue = new List<Action>();
        private readonly List<Action> _fixedUpdateQueue = new List<Action>();
        private readonly List<Action> _lateUpdateQueue = new List<Action>();
        
        private static void ValidateSignal<T>(Enum key, Dictionary<Enum, ISignal> signals)
        {
            if (!signals.ContainsKey(key)) return;
            try
            {
                T signal = (T)signals[key];
            }
            catch (InvalidCastException e)
            {
                throw new Exception($"Attempting to subscribe '{key}' but was already subscribed resulting in '{e}'");
            }
        }

        private static T GetSignal<T>(Enum key, Dictionary<Enum, ISignal> signals) where T : ISignal
        {
            if (!signals.ContainsKey(key)) signals.Add(key, (ISignal)Activator.CreateInstance(typeof(T)));
            return (T)(signals[key]);
        }

        private static bool TryGetSignal<T>(Enum key, Dictionary<Enum, ISignal> signals, out T signal) where T : ISignal
        {
            bool output = signals.ContainsKey(key);
            if (!output) signals.Add(key, (ISignal)Activator.CreateInstance(typeof(T)));
            signal = (T)signals[key];
            return output;
        }

        public void Clear()
        {
            _signals.Clear();
            _signalsOnce.Clear();
        }

        public void Subscribe(Enum key, Action handler, TelegraphStyles style)
        {
            switch (style)
            {
                case TelegraphStyles.Normal:
                    ValidateSignal<Signal>(key, _signals);
                    GetSignal<Signal>(key, _signals).Subscribe(handler);
                    break;
                case TelegraphStyles.Once:
                    ValidateSignal<Signal>(key, _signalsOnce);
                    GetSignal<Signal>(key, _signalsOnce).Subscribe(handler);
                    break;
                case TelegraphStyles.Permanent:
                    ValidateSignal<Signal>(key, _signalsPermanent);
                    GetSignal<Signal>(key, _signalsPermanent).Subscribe(handler);
                    break;
            }
        }

        public void Subscribe<TMsg>(Enum key, Action<TMsg> handler, TelegraphStyles style)
        {
            switch (style)
            {
                case TelegraphStyles.Normal:
                    ValidateSignal<Signal>(key, _signals);
                    GetSignal<Signal<TMsg>>(key, _signals).Subscribe(handler);
                    break;
                case TelegraphStyles.Once:
                    ValidateSignal<Signal>(key, _signalsOnce);
                    GetSignal<Signal<TMsg>>(key, _signalsOnce).Subscribe(handler);
                    break;
                case TelegraphStyles.Permanent:
                    ValidateSignal<Signal>(key, _signalsPermanent);
                    GetSignal<Signal<TMsg>>(key, _signalsPermanent).Subscribe(handler);
                    break;
            }
        }

        public void Unsubscribe(Enum key, Action handler)
        {
            GetSignal<Signal>(key, _signals).Unsubscribe(handler);
            GetSignal<Signal>(key, _signalsOnce).Unsubscribe(handler);
            GetSignal<Signal>(key, _signalsPermanent).Unsubscribe(handler);
        }
        public void Unsubscribe<TMsg>(Enum key, Action<TMsg> handler)
        {
            GetSignal<Signal<TMsg>>(key, _signals).Unsubscribe(handler);
            GetSignal<Signal<TMsg>>(key, _signals).Unsubscribe(handler);
            GetSignal<Signal<TMsg>>(key, _signals).Unsubscribe(handler);
        }

        public void Send(Enum key, TelegraphModes mode)
        {
            switch (mode)
            {
                case TelegraphModes.Immediate:
                    InternalSend(key);
                    break;
                case TelegraphModes.OnUpdate:
                    _updateQueue.Add(() => { InternalSend(key); });
                    break;
                case TelegraphModes.OnFixedUpdate:
                    _fixedUpdateQueue.Add(() => { InternalSend(key); });
                    break;
                case TelegraphModes.OnLateUpdate:
                    _lateUpdateQueue.Add(() => { InternalSend(key); });
                    break;
            }
        }

        private void InternalSend(Enum key)
        {
            Signal signal;
            if (TryGetSignal(key, _signalsPermanent, out signal)) signal.Send();
            if (TryGetSignal(key, _signals, out signal)) signal.Send();
            if (TryGetSignal(key, _signalsOnce, out signal))
            {
                _signalsOnce.Remove(key);
                signal.Send();
            }
        }
        
        public void Send<TMsg>(Enum key, TMsg msg, TelegraphModes mode)
        {
            switch (mode)
            {
                case TelegraphModes.Immediate:
                    InternalSend(key, msg);
                    break;
                case TelegraphModes.OnUpdate:
                    _updateQueue.Add(() => { InternalSend(key, msg); });
                    break;
                case TelegraphModes.OnFixedUpdate:
                    _fixedUpdateQueue.Add(() => { InternalSend(key, msg); });
                    break;
                case TelegraphModes.OnLateUpdate:
                    _lateUpdateQueue.Add(() => { InternalSend(key, msg); });
                    break;
            }
        }

        private void InternalSend<TMsg>(Enum key, TMsg msg)
        {
            Signal<TMsg> signal;
            if (TryGetSignal(key, _signalsPermanent, out signal)) signal.Send(msg);
            if (TryGetSignal(key, _signals, out signal)) signal.Send(msg);
            if (TryGetSignal(key, _signalsOnce, out signal))
            {
                _signalsOnce.Remove(key);
                signal.Send(msg);
            }
        }

        public void ProcessEvents(TelegraphModes mode)
        {
            switch (mode)
            {
                case TelegraphModes.OnUpdate:
                    foreach (Action callback in _updateQueue) callback();
                    _updateQueue.Clear();
                    break;
                case TelegraphModes.OnFixedUpdate:
                    foreach (Action callback in _fixedUpdateQueue) callback();
                    _fixedUpdateQueue.Clear();
                    break;
                case TelegraphModes.OnLateUpdate:
                    foreach (Action callback in _lateUpdateQueue) callback();
                    _lateUpdateQueue.Clear();
                    break;
            }
        }
    }
}