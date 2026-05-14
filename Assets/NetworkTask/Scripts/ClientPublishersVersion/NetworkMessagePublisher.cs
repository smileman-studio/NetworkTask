using System;
using Mirror;

namespace NetworkTask.ClientPublishersVersion
{
    public class NetworkMessagePublisher<T> : INetworkMessagePublisher where T : struct, NetworkMessage
    {
        private Action<T> handler;

        public ushort MessageId => NetworkMessageId<T>.Id;
        public bool HasSubscribers => handler != null;

        public void Subscribe<TU>(Action<TU> subscriber) where TU : NetworkMessage
        {
            if (subscriber is Action<T> action)
            {
                handler += action;
            }
        }

        public void Unsubscribe<TU>(Action<TU> subscriber) where TU : NetworkMessage
        {
            if (subscriber is Action<T> action)
            {
                handler -= action;
            }
        }

        public void RegisterHandler()
        {
            NetworkClient.RegisterHandler(handler);
        }
    }
}