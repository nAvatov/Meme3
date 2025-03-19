using _ProjectAssets.Scripts.FSM;
using _ProjectAssets.Scripts.FSM.Game_States;
using _ProjectAssets.Scripts.Game_States;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class StateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PrimaryGenerationState>().AsSingle().NonLazy();
            Container.Bind<CheckMatchesState>().AsSingle().NonLazy();
            Container.Bind<MoveMatchesState>().AsSingle().NonLazy();
            Container.Bind<DestroyMatchedElementsState>().AsSingle().NonLazy();
            Container.Bind<DropMatchesState>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<FSMachine>().AsSingle();
        }
    }
}