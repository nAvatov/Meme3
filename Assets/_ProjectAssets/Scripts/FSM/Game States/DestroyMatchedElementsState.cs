using System;
using System.Collections.Generic;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class DestroyMatchedElementsState : GameState
    {
        private FSMachine _fsm;
        private GameFieldView _gameFieldView;
        private StateTransitionContext _transitionContext;
        private System.Random _rnd;
        
        [Inject]
        public void Construct(FSMachine fsm, GameFieldView gameFieldView, StateTransitionContext transitionContext, System.Random rnd)
        {
            _gameFieldView = gameFieldView;
            _transitionContext = transitionContext;
            _fsm = fsm;
            _rnd = rnd;
        }
        public override void Enter()
        {
            if (_transitionContext.MovedElement && _transitionContext.TargetedElement)
            {
                _gameFieldView.ChangeInteractabilityOfSwapElements(_transitionContext.MovedElement.PositionData, _transitionContext.TargetedElement.PositionData,true);
            }
            
            ExplodeMatches(_transitionContext.VerticalMatches);
            ExplodeMatches(_transitionContext.HorizontalMatches);
            
            _transitionContext.ClearCachedSwapMatches();
            
            _fsm.ChangeState<DropMatchesState>();
        }

        private void ExplodeMatches(List<List<ArrayPositionData>> matches)
        {
            int upperRandomizeBound = Enum.GetValues(typeof(ElementType)).Length;
            
            foreach (var match in matches)
            {
                // Display at ui layer
                _transitionContext.ComboAmount += match.Count;
                
                foreach (var element in match)
                {
                    if (_gameFieldView.MatchElements[element.RowIndex, element.ColumnIndex])
                    {
                        _gameFieldView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                        
                        _gameFieldView.ReturnElementToSpawnPoint(element.RowIndex, element.ColumnIndex);
                        
                        SetNewElementData(element.RowIndex, element.ColumnIndex, upperRandomizeBound);
                    }
                }
            }
        }

        private void SetNewElementData(int rowIndex, int columnIndex, int upperRandomizeBound)
        {
            _gameFieldView.ReservedElements[rowIndex, columnIndex] = _gameFieldView.MatchElements[rowIndex, columnIndex];
            _gameFieldView.ReservedElements[rowIndex, columnIndex].SetElementType((ElementType)_rnd.Next(0, upperRandomizeBound));
            _gameFieldView.MatchElements[rowIndex, columnIndex] = null;
        }

        public override void Exit()
        {
            _transitionContext.ComboAmount = 0;
        }
    }
}