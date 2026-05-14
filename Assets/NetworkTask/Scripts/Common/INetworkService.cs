using System;
using Mirror;

namespace NetworkTask.Common
{
    public interface INetworkService
    {
        event Action OnClientConnectedEvent;
        event Action OnClientDisconnectedEvent;
        void Subscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage;
        void Unsubscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage;
        void Send<T>(T message) where T : struct, NetworkMessage;
    }
}