using Mirror;

namespace NetworkTask.Common
{
    public struct UnsubscribeMessage : NetworkMessage 
    { 
        public ushort MessageId; 
    }
}