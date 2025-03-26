using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class MoveMatchesState : GameState
    {
        private FSMachine _fsm;
        private SignalBus _signalBus;
        private GameFieldView _gameFieldView;
        private StateTransitionContext _transitionContext;

        private UniTask _primarySwapTask;
        
        [Inject]
        public void Construct(FSMachine fsm, GameFieldView gameFieldView, SignalBus signalBus, StateTransitionContext transitionContext)
        {
            _fsm = fsm;
            _gameFieldView = gameFieldView;
            _signalBus = signalBus;
            _transitionContext = transitionContext;
            
            _signalBus.Subscribe<SwapSignal>(SwapSignalHandler);
        }
        public override async void Enter()
        {
            if (_transitionContext.MovedElement && _transitionContext.TargetedElement)
            {
                await ReturnSwapElementToPreviousPos();
                _gameFieldView.ChangeInteractabilityOfSwapElements(_transitionContext.MovedElement.PositionData, _transitionContext.TargetedElement.PositionData,true);
            }
            _gameFieldView.HandleFieldInteractability(true);
            Debug.Log("Move state");
        }

        public override void Exit()
        {
            _gameFieldView.HandleFieldInteractability(false);
        }

        private void SwapSignalHandler(SwapSignal signal)
        {
            ProceedSwapAsync(signal).Forget();
        }

        private async UniTaskVoid ProceedSwapAsync(SwapSignal signal)
        {
            if (IsSwapConsiderRule(signal.MovingElement, signal.TargetElement))
            {
                _transitionContext.CacheSwappedMatches(
                    _gameFieldView.MatchElements[signal.MovingElement.RowIndex, signal.MovingElement.ColumnIndex], 
                    _gameFieldView.MatchElements[signal.TargetElement.RowIndex, signal.TargetElement.ColumnIndex]
                );
                
                _gameFieldView.ChangeInteractabilityOfSwapElements(signal.MovingElement, signal.TargetElement, false);
                
                await _gameFieldView.Swap(signal.MovingElement, signal.TargetElement);
                
                SwapElementsData(signal.MovingElement, signal.TargetElement);
                
                _fsm.ChangeState<CheckMatchesState>();
            }
            else
            {
                _gameFieldView.MatchElements[signal.MovingElement.RowIndex, signal.MovingElement.ColumnIndex].GetComponent<DragAndDrop>().ReturnElementToInitialPosition();
                _gameFieldView.MatchElements[signal.MovingElement.RowIndex, signal.MovingElement.ColumnIndex].GetComponent<DragAndDrop>().ChangeInteractability(true);
            }
        }

        private bool IsSwapConsiderRule(ArrayPositionData el1, ArrayPositionData el2)
        {
            return 
                (_gameFieldView.MatchElements[el1.RowIndex, el1.ColumnIndex].ElementType != _gameFieldView.MatchElements[el2.RowIndex, el2.ColumnIndex].ElementType) &&
                
                ((el1.RowIndex == el2.RowIndex + 1 && el1.ColumnIndex == el2.ColumnIndex) ||
                 (el1.RowIndex == el2.RowIndex - 1 && el1.ColumnIndex == el2.ColumnIndex) || 
                 (el1.ColumnIndex == el2.ColumnIndex + 1 && el1.RowIndex == el2.RowIndex) ||
                 (el1.ColumnIndex == el2.ColumnIndex - 1 && el1.RowIndex == el2.RowIndex));
        }

        private void SwapElementsData(ArrayPositionData el1, ArrayPositionData el2)
        {
            MatchElement buf;
            
            buf = _gameFieldView.MatchElements[el2.RowIndex, el2.ColumnIndex];
            _gameFieldView.MatchElements[el2.RowIndex,el2.ColumnIndex] =_gameFieldView.MatchElements[el1.RowIndex, el1.ColumnIndex];
            _gameFieldView.MatchElements[el1.RowIndex, el1.ColumnIndex] = buf;
            
            
            _gameFieldView.MatchElements[el1.RowIndex, el1.ColumnIndex].SetPositionData(el1);
            _gameFieldView.MatchElements[el2.RowIndex, el2.ColumnIndex].SetPositionData(el2);
        }
        

        private async UniTask ReturnSwapElementToPreviousPos()
        {
            await _gameFieldView.Swap(
                _transitionContext.MovedElement.PositionData, 
                _transitionContext.TargetedElement.PositionData
            );
            
            SwapElementsData(_transitionContext.MovedElement.PositionData, _transitionContext.TargetedElement.PositionData);
        }
    }
}