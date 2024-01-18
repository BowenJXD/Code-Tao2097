namespace CodeTao
{
    public interface IWAtSource
    {
        BindableStat GetWAt(EWAt at);
    }
    
    public interface IWAtReceiver
    {
        void Receive(IWAtSource source);
    }
}