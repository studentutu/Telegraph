namespace RedOwl.Engine
{
    public class AwakeDestroyBinder : EventBinder
    {
        private void Awake() => Subscribe();
        private void OnDestroy() => Unsubscribe();
    }
}