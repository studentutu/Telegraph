namespace RedOwl.Engine
{
    public class EnableDisableBinder : EventBinder
    {
        private void OnEnable() => Subscribe();
        private void OnDisable() => Unsubscribe();
    }
}