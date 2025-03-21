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
        private GridView _gridView;
        private StateTransitionContext _transitionContext;
        private System.Random _rnd;
        
        [Inject]
        public void Construct(FSMachine fsm, GridView gridView, StateTransitionContext transitionContext, System.Random rnd)
        {
            _gridView = gridView;
            _transitionContext = transitionContext;
            _fsm = fsm;
            _rnd = rnd;
        }
        public override async void Enter()
        {
            ExplodeMatches(_transitionContext.VerticalMatches);
            ExplodeMatches(_transitionContext.HorizontalMatches);
            //await UniTask.Delay(3000);
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
                    if (_gridView.MatchElements[element.RowIndex, element.ColumnIndex] != null)
                    {
                        _gridView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                        _gridView.ReturnElementToSpawnPoint(element.RowIndex, element.ColumnIndex, (ElementType)_rnd.Next(0, upperRandomizeBound));
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