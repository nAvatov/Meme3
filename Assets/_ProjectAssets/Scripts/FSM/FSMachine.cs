using System.Collections.Generic;
using System.Linq;
using _ProjectAssets.Scripts.FSM.Game_States;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM
{
    public class FSMachine : IInitializable
    {
        private readonly StateFactory _stateFactory;
        private  List<GameState> _gameStates;
        private GameState _currentState;

        public FSMachine(StateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public void Initialize()
        {
            _gameStates = new List<GameState>()
            {
                _stateFactory.CreateState<PrimaryGenerationState>(),
                _stateFactory.CreateState<CheckMatchesState>(),
                _stateFactory.CreateState<MoveMatchesState>(),
                _stateFactory.CreateState<DropMatchesState>(),
                _stateFactory.CreateState<DestroyMatchedElementsState>()
            };
        
            ChangeState<PrimaryGenerationState>();
            Debug.Log("Entered FSM");
        }

        public void ChangeState<T>()
        {
            if (_currentState != null && typeof(T) == _currentState.GetType())
            {
                return;
            }
        
            _currentState?.Exit();
        
            _currentState = _gameStates.OfType<T>().First() as GameState;
        
            if (_currentState != null)
            {
                _currentState.Enter();
            }
        }
    }
}
