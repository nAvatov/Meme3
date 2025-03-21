using _ProjectAssets.Scripts.View;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class MoveMatchesState : GameState
    {
        private FSMachine _fsm;
        private GameFieldView _gameFieldView;

        [Inject]
        public void Construct(FSMachine fsm, GameFieldView gameFieldView)
        {
            _fsm = fsm;
            _gameFieldView = gameFieldView;
        }
        public override void Enter()
        {
            _gameFieldView.HandleFieldInteractability(true);
            
            
        }
        
    }
}