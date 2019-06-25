namespace Datatec.Infrastructure
{
    public interface ISlackClient
    {
        void PostMessage(Payload payload);
        void PostMessage(string text);
    }
}