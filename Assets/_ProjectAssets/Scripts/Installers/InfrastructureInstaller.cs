using _ProjectAssets.Scripts.FSM;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class InfrastructureInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StateFactory>().AsSingle();
            Container.Bind<StateTransitionContext>().AsSingle();
        }
    }
}