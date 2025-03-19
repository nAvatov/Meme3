using System.Collections.Generic;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Structures;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class DestroyMatchedElementsState : GameState
    {
        private GridView _gridView;
        private StateTransitionContext _transitionContext;
        
        [Inject]
        public void Construct(GridView gridView, StateTransitionContext transitionContext)
        {
            _gridView = gridView;
            _transitionContext = transitionContext;
        }
        public override void Enter()
        {
            ExplodeMatches(_transitionContext.VerticalMatches);
            ExplodeMatches(_transitionContext.HorizontalMatches);
        }

        private void ExplodeMatches(List<List<ArrayPositionData>> matches)
        {
            foreach (var match in matches)
            {
                _transitionContext.ComboAmount += match.Count;
                // Display at ui layer
                foreach (var element in match)
                {
                    _gridView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                }
            }
        }

        public override void Exit()
        {
            _transitionContext.ComboAmount = 0;
        }

        public override void SetNextState()
        {
            
        }
    }
}