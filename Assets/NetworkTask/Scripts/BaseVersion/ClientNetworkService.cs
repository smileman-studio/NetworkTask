using System;
using Mirror;
using NetworkTask.Common;
using VContainer.Unity;

namespace NetworkTask.BaseVersion
{
    public class ClientNetworkService : IStartable, INetworkService, IDisposable
    {
        private readonly SimpleNetworkManager networkManager;

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
            OnClientConnectedEvent?.Invoke();
        }

        private void ClientDisconnectedHandler()
        {
            OnClientDisconnectedEvent?.Invoke();
        }

        public void Subscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage
        {
            ushort messageId = NetworkMessageId<T>.Id;
            NetworkClient.RegisterHandler(subscriber);
            NetworkClient.Send(new SubscribeMessage { MessageId = messageId });
        }

        public void Unsubscribe<T>(Action<T> subscriber) where T : struct, NetworkMessage
        {
            ushort messageId = NetworkMessageId<T>.Id;
            if (NetworkClient.active)
            {
                NetworkClient.Send(new UnsubscribeMessage() { MessageId = messageId });
            }
            NetworkClient.UnregisterHandler<T>();
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