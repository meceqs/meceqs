namespace Meceqs.Internal
{
    public interface IHasContextData
    {
        T GetContextItem<T>(string key);

        void SetContextItem(string key, object value);
    }
}