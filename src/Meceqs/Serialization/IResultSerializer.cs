namespace Meceqs.Serialization
{
    public interface IResultSerializer
    {
        string ContentType { get; }

        string SerializeResultToString(object result);
    }
}