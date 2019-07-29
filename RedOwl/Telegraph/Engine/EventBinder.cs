using System;
using UnityEngine;

namespace RedOwl.Engine
{
    public class EventBinder : MonoBehaviour
    {
        private Action<bool> _binder;
        
        #if REDOWL_USE_TELEGRAPH_JOBS
        
        internal void Bind<TJob>(Enum key, Action handler, TelegraphStyles style)
        {
            _binder = (bool active) =>
            {
                if (active) Telegraph.Subscribe<TJob>(key, handler, style);
                else Telegraph.Unsubscribe<TJob>(key, handler);
            };
            _binder(true);
        }

        internal void Bind<TMsg, TJob>(Enum key, Action handler, TelegraphStyles style)
        {
            _binder = (bool active) =>
            {
                if (active) Telegraph.Subscribe<TMsg, TJob>(key, handler, style);
                else Telegraph.Unsubscribe<TMsg, TJob>(key, handler);
            };
            _binder(true);
        }
        
        #else
        
        internal void Bind(Enum key, Action handler, TelegraphStyles style)
        {
            _binder += active =>
            {
                if (active) Telegraph.Subscribe(key, handler, style);
                else Telegraph.Unsubscribe(key, handler);
            };
            Telegraph.Subscribe(key, handler, style);
        }

        internal void Bind<TMsg>(Enum key, Action<TMsg> handler, TelegraphStyles style)
        {
            _binder += active =>
            {
                if (active) Telegraph.Subscribe(key, handler, style);
                else Telegraph.Unsubscribe(key, handler);
            };
            Telegraph.Subscribe(key, handler, style);
        }
        
        #endif

        internal void Subscribe() => _binder?.Invoke(true);

        internal void Unsubscribe() => _binder?.Invoke(false);
    }
}