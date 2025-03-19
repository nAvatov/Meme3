using _ProjectAssets.Scripts.FSM;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class InfrastructureInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StateFactory>().AsSingle();
        }
    }
}