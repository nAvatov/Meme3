using _ProjectAssets.Scripts.FSM;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class InfrastructureInstaller : MonoInstaller
    {
        private System.Random _generationRnd = new System.Random();
        public override void InstallBindings()
        {
            Container.Bind<StateFactory>().AsSingle();
            Container.Bind<StateTransitionContext>().AsSingle();
            Container.BindInstance(_generationRnd).AsSingle();
        }
    }
}