namespace Meceqs.Serialization
{
    public interface IEnvelopeSerializer
    {
        string ContentType { get; }

        byte[] SerializeToByteArray(Envelope envelope);

        string SerializeToString(Envelope envelope);
    }
}