using TMPro.EditorUtilities;
using Zenject;

namespace _ProjectAssets.Scripts.FSM
{
    public class StateFactory
    {
        private readonly DiContainer _container;
        
        public StateFactory(DiContainer container) => _container = container;

        public T CreateState<T>() where T : GameState
        {
            return _container.Resolve<T>();
        }
    }
}