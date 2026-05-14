using System.Collections.Generic;
using Mirror;
using UnityEngine;
using VContainer.Unity;

namespace NetworkTask.Common
{
    public class ServerNetworkService : IStartable
    {
        private readonly SimpleNetworkManager networkManager;
        private ushort helloMessageId;
        private readonly Dictionary<ushort, HashSet<int>> subscribers = new();

        public ServerNetworkService(SimpleNetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public void Start()
        {
            helloMessageId = NetworkMessageId<HelloMessage>.Id;
            networkManager.OnStartServerEvent += StartServerHandler;
        }

        private void StartServerHandler()
        {
            Debug.Log($"StartServerHandler");
            NetworkServer.RegisterHandler<SubscribeMessage>(ClientSubscribeHandler);
            NetworkServer.RegisterHandler<UnsubscribeMessage>(ClientUnsubscribeHandler);
        }

        private void ClientSubscribeHandler(NetworkConnectionToClient connection, SubscribeMessage message)
        {
            if (!subscribers.ContainsKey(message.MessageId))
            {
                subscribers[message.MessageId] = new HashSet<int>();
            }

            subscribers[message.MessageId].Add(connection.connectionId);
            HelloMessageSubscribeHandler(connection, message);
        }

        private void ClientUnsubscribeHandler(NetworkConnectionToClient connection, UnsubscribeMessage message)
        {
            if (subscribers.TryGetValue(message.MessageId, out var hashSet))
                hashSet.Remove(connection.connectionId);
        }

        private void HelloMessageSubscribeHandler(NetworkConnectionToClient connection, SubscribeMessage message)
        {
            if (message.MessageId == helloMessageId)
            {
                SendMessageToSubscriber(connection.connectionId, new HelloMessage() { Text = "Hello Client!" });
            }
        }

        public void SendMessageToSubscriber<T>(int connectionId, T message)
            where T : struct, NetworkMessage
        {
            var messageId = NetworkMessageId<T>.Id;
            if (!subscribers.TryGetValue(messageId, out var hashSet))
                return;
            if (!hashSet.Contains(connectionId))
                return;
            if (NetworkServer.connections.TryGetValue(connectionId, out var connection))
                connection.Send(message);
        }
    }
}