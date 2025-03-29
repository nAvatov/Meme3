using _ProjectAssets.Scripts.FSM;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Structures;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class InfrastructureInstaller : MonoInstaller
    {
        private System.Random _generationRnd = new System.Random();

        public override void InstallBindings()
        {
            Container.BindInstance(_generationRnd).AsSingle();
            
            InstallStateInfrastructure();
            InstallSignals();
        }

        public void InstallSignals()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<SwapSignal>();
        }

        private void InstallStateInfrastructure()
        {
            Container.Bind<StateFactory>().AsSingle();
            Container.Bind<StateTransitionContext>().AsSingle();
        }
    
    }
}