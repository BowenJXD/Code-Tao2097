using System.Collections.Generic;

namespace CodeTao
{
    public interface IAAtSource
    {
        BindableStat GetAAt(EAAt at);
        
        void Transmit(List<IAAtReceiver> receivers)
        {
            foreach(IAAtReceiver receiver in receivers)
            {
                receiver.Receive(this);
            }
        }
    }
    
    public interface IAAtReceiver
    {
        void Receive(IAAtSource source);
    }
}