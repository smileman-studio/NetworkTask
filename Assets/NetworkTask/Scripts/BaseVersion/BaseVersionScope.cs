using NetworkTask.Common;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace NetworkTask.BaseVersion
{
    public class BaseVersionScope : LifetimeScope
    {
        [SerializeField] private SimpleNetworkManager networkManager;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(networkManager);
        
            builder.Register<ServerNetworkService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ClientNetworkService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<HelloService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}