using System;
using System.Collections.Generic;
using Mirror;
using NetworkTask.Common;
using VContainer.Unity;

namespace NetworkTask.ClientPublishersVersion
{
    public class ClientNetworkService : IStartable, INetworkService, IDisposable
    {
        private readonly SimpleNetworkManager networkManager;
        private readonly Dictionary<ushort, INetworkMessagePublisher> publishers = new();

        public event Action OnClientConnectedEvent;
        public event Action OnClientDisconnectedEvent;

        public ClientNetworkService(SimpleNetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public void Start()
        {
            networkManager.OnClientConnectedEvent += ClientConnectedHandler;
            networkManager.OnClientDisconnectedEvent += ClientDisconnectedHandler;
        }

        private void ClientConnectedHandler()
        {
            foreach (var publisher in publishers.Values)
            {
                if (publisher.HasSubscribers)
                {
                    publisher.RegisterHandler();
                    NetworkClient.Send(new SubscribeMessage { MessageId = publisher.MessageId });
                }
            }

            OnClientConnectedEvent?.Invoke();
        }

        private void ClientDisconnectedHandler()
        {
            OnClientDisconnectedEvent?.Invoke();
        }

        public void Subscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage
        {
            ushort messageId = NetworkMessageId<T>.Id;
            if (!publishers.TryGetValue(messageId, out var publisher))
            {
                publisher = new NetworkMessagePublisher<T>();
                publishers.Add(messageId, publisher);
            }

            if (!publisher.HasSubscribers && NetworkClient.active)
            {
                publisher.RegisterHandler();
                NetworkClient.Send(new SubscribeMessage { MessageId = messageId });
            }

            publisher.Subscribe(subscriber);
        }

        public void Unsubscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage
        {
            ushort messageId = NetworkMessageId<T>.Id;
            if (!publishers.TryGetValue(messageId, out var publisher))
            {
                return;
            }

            publisher.Unsubscribe(subscriber);
            if (!publisher.HasSubscribers)
            {
                if (NetworkClient.active)
                    NetworkClient.Send(new UnsubscribeMessage() { MessageId = messageId });
                NetworkClient.UnregisterHandler<T>();
            }
        }

        public void Send<T>(T message) where T : struct, NetworkMessage
        {
            NetworkClient.Send(message);
        }

        public void Dispose()
        {
            networkManager.OnClientConnectedEvent -= ClientConnectedHandler;
            networkManager.OnClientDisconnectedEvent -= ClientDisconnectedHandler;
        }
    }
}