using System;
using System.Collections.Generic;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
                _transitionContext.ComboAmount += match.Count;
                // Display at ui layer
                foreach (var element in match)
                {
                    if (_gameFieldView.MatchElements[element.RowIndex, element.ColumnIndex] != null)
                    {
                        _gameFieldView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                        _gameFieldView.ReturnElementToSpawnPoint(element.RowIndex, element.ColumnIndex, (ElementType)_rnd.Next(0, upperRandomizeBound));
                    }
                }
            }
        }

        public override void Exit()
        {
            _transitionContext.ComboAmount = 0;
        }
    }
}