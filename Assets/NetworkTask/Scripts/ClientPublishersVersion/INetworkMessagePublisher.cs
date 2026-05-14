using System;
using Mirror;

namespace NetworkTask.ClientPublishersVersion
{
    public interface INetworkMessagePublisher
    {
        ushort MessageId { get; }
        bool HasSubscribers { get; }
        void Subscribe<T>(Action<T> subscriber) where T : NetworkMessage;
        void Unsubscribe<T>(Action<T> subscriber) where T : NetworkMessage;
        void RegisterHandler();
    }
}