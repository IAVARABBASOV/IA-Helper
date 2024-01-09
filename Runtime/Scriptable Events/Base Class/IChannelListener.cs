namespace IA.ScriptableEvent.Listener
{
    public interface IChannelListener<T>
    {
        void InvokeResponse(T targetType);
    }
}