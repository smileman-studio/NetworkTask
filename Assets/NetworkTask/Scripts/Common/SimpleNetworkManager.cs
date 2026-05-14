using System;
using Mirror;

namespace NetworkTask.Common
{
    public class SimpleNetworkManager : NetworkManager
    {
        public event Action OnStartServerEvent;
        public event Action OnClientConnectedEvent; 
        public event Action OnClientDisconnectedEvent;
    
        public override void OnStartServer()
        {
            base.OnStartServer();
            OnStartServerEvent?.Invoke();
        }
    
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            OnClientConnectedEvent?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnectedEvent?.Invoke();
        }
    }
}