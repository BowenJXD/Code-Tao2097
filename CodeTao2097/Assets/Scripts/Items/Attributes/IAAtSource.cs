namespace CodeTao
{
    public interface IAAtSource
    {
        BindableStat GetAAt(EAAt at);
    }
    
    public interface IAAtReceiver
    {
        void Receive(IAAtSource source);
    }
}