using System.Collections.Generic;

namespace CodeTao
{
    public interface IWAtSource
    {
        BindableStat GetWAt(EWAt at);

        void Transmit(IWAtReceiver[] receivers)
        {
            foreach(IWAtReceiver receiver in receivers)
            {
                receiver.Receive(this);
            }
        }
    }
    
    public interface IWAtReceiver
    {
        void Receive(IWAtSource source);
    }
}