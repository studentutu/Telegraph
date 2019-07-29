using System;
using UnityEngine;

namespace RedOwl.Engine
{
    public enum TelegraphStyles
    {
        Normal,
        Once,
        Permanent
    }

    public enum TelegraphModes
    {
        Immediate,
        OnUpdate,
        OnFixedUpdate,
        OnLateUpdate
    }

    public static class Telegraph
    {
        private static TelegraphHelper _helper;

        private static readonly EventsManager Manager = new EventsManager();

        public static void Subscribe(Enum key, Action handler, TelegraphStyles style = TelegraphStyles.Normal) => Manager.Subscribe(key, handler, style);
        public static void Subscribe<TMsg>(Enum key, Action<TMsg> handler, TelegraphStyles style = TelegraphStyles.Normal) => Manager.Subscribe<TMsg>(key, handler, style);

        public static void Unsubscribe(Enum key, Action handler) => Manager.Unsubscribe(key, handler);
        public static void Unsubscribe<TMsg>(Enum key, Action<TMsg> handler) => Manager.Unsubscribe(key, handler);

        public static void BindEventUntilDisable(this MonoBehaviour self, Enum key, Action handler, TelegraphStyles style = TelegraphStyles.Normal) => self.gameObject.GetOrAddComponent<EnableDisableBinder>().Bind(key, handler, style);
        public static void BindEventUntilDisable<TMsg>(this MonoBehaviour self, Enum key, Action<TMsg> handler, TelegraphStyles style = TelegraphStyles.Normal) => self.gameObject.GetOrAddComponent<EnableDisableBinder>().Bind<TMsg>(key, handler, style);

        public static void BindEventUntilDestroy(this MonoBehaviour self, Enum key, Action handler, TelegraphStyles style = TelegraphStyles.Normal) => self.gameObject.GetOrAddComponent<AwakeDestroyBinder>().Bind(key, handler, style);
        public static void BindEventUntilDestroy<TMsg>(this MonoBehaviour self, Enum key, Action<TMsg> handler, TelegraphStyles style = TelegraphStyles.Normal) => self.gameObject.GetOrAddComponent<AwakeDestroyBinder>().Bind(key, handler, style);
        
        public static void Clear() => Manager.Clear();

        public static void Send(Enum key, TelegraphModes mode = TelegraphModes.Immediate) => Manager.Send(key, mode);

        public static void Send<TMsg>(Enum key, TMsg msg, TelegraphModes mode = TelegraphModes.Immediate) => Manager.Send<TMsg>(key, msg, mode);
        
        internal static void ProcessEvents(TelegraphModes mode) => Manager.ProcessEvents(mode);

        private static T GetOrAddComponent<T>(this GameObject self) where T : EventBinder
        {
            T component = self.GetComponent<T>();
            if (null == component) component = self.AddComponent<T>();
            return component;
        }

        public static void SendEvent(this MonoBehaviour self, Enum key, TelegraphModes mode = TelegraphModes.Immediate) => Manager.Send(key, mode);

        public static void SendEvent<TMsg>(this MonoBehaviour self, Enum key, TMsg msg, TelegraphModes mode = TelegraphModes.Immediate) => Manager.Send(key, msg, mode);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _helper = new GameObject("Telegraph Helper").AddComponent<TelegraphHelper>();
        }
    }
}