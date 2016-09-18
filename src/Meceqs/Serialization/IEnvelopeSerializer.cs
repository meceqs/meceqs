namespace Meceqs.Serialization
{
    public interface IEnvelopeSerializer
    {
        string ContentType { get; }

        byte[] SerializeEnvelopeToByteArray(Envelope envelope);

        string SerializeEnvelopeToString(Envelope envelope);
    }
}