using System;
using NetworkTask.Common;
using UnityEngine;
using VContainer.Unity;

namespace NetworkTask.ClientPublishersVersion
{
    public class HelloService : IStartable, IDisposable
    {
        private readonly INetworkService networkService;

        public HelloService(INetworkService networkService)
        {
            this.networkService = networkService;
        }
    
        public void Start()
        {
            SubscribeMessages();
        }

        private void SubscribeMessages()
        {
            networkService.Subscribe<HelloMessage>(HelloMessageHandler);
        }

        private void UnsubscribeMessages()
        {
            networkService.Unsubscribe<HelloMessage>(HelloMessageHandler);
        }
    
        private void HelloMessageHandler(HelloMessage message)
        {
            Debug.Log($"Server message: {message.Text}");
        }

        public void Dispose()
        {
            UnsubscribeMessages();
        }
    }
}