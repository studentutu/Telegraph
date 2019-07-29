using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedOwl.Engine
{
    public sealed class TelegraphHelper : MonoBehaviour {
        private void Awake() => DontDestroyOnLoad(gameObject);

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single) Telegraph.Clear();
        }

        private void Update() => Telegraph.ProcessEvents(TelegraphModes.OnUpdate);
        private void FixedUpdate() => Telegraph.ProcessEvents(TelegraphModes.OnFixedUpdate);
        private void LateUpdate() => Telegraph.ProcessEvents(TelegraphModes.OnLateUpdate);
    }
}