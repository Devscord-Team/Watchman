namespace Watchman.Cqrs
{
    public interface IMessage : IMessage<EmptyResult>
    {
    }

    public interface IMessage<out T>
    {
    }

    public class EmptyResult
    {
        private EmptyResult()
        {
        }

        public static readonly EmptyResult Value = new EmptyResult();
    }
}
