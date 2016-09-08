namespace Meceqs.Serialization
{
    public interface IResultSerializer
    {
        string ContentType { get; }

        string SerializeToString(object result);
    }
}