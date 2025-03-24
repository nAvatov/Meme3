using _ProjectAssets.Scripts.Installers;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class MoveMatchesState : GameState
    {
        private FSMachine _fsm;
        private SignalBus _signalBus;
        private GameFieldView _gameFieldView;
        

        [Inject]
        public void Construct(FSMachine fsm, GameFieldView gameFieldView, SignalBus signalBus)
        {
            _fsm = fsm;
            _gameFieldView = gameFieldView;
            _signalBus = signalBus;
            
            _signalBus.Subscribe<SwapSignal>(ProceedSwap);
            Debug.Log(signalBus);
        }
        public override void Enter()
        {
            Debug.Log("Move state entered");
            _gameFieldView.HandleFieldInteractability(true);
        }

        private void ProceedSwap(SwapSignal signal)
        {
            Debug.Log("Trying to swap object " + signal.MovingElement.RowIndex + " " + signal.MovingElement.ColumnIndex + " with " + signal.TargetElement.RowIndex + " " + signal.TargetElement.ColumnIndex);
        }
        
    }
}