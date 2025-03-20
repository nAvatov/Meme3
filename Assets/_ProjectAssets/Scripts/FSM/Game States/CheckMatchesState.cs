using System.Collections.Generic;
using System.Linq;
using _ProjectAssets.Scripts.FSM.States_Infrastructure;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class CheckMatchesState : GameState
    {
        private GridView _gridView;
        private FSMachine _fsm;
        private StateTransitionContext _transitionContext;

        [Inject]
        public void Construct(GridView gridView, FSMachine fsm, StateTransitionContext transitionContext)
        {
            _gridView = gridView;
            _fsm = fsm;
            _transitionContext = transitionContext;
        }

        public override async void Enter()
        {
            var foundMatches = await IsMatchFound();
            if (foundMatches)
            {
                Debug.Log("Matches found!");
                
                _fsm.ChangeState<DestroyMatchedElementsState>();
            }
            else
            {
                _fsm.ChangeState<MoveMatchesState>();
            }
        }

        private async UniTask<bool> IsMatchFound()
        {
            List<List<ArrayPositionData>> verticalMatchesList = new List<List<ArrayPositionData>>();
            List<List<ArrayPositionData>> horizontalMatchesList = new List<List<ArrayPositionData>>();
            
            for (int i = 0; i < _gridView.MatchElements.GetLength(0); i++)
            {
                for (int j = 0; j < _gridView.MatchElements.GetLength(1); j++)
                {
                    if (!IsElementWasIncludedInMatchBefore(verticalMatchesList, i, j))
                    {
                        verticalMatchesList.Add(CheckMatchFigureVertical(i, j));
                    }

                    if (!IsElementWasIncludedInMatchBefore(horizontalMatchesList, i, j))
                    {
                        horizontalMatchesList.Add(CheckMatchFigureHorizontal(i, j));
                    }
                }
            }

            verticalMatchesList = verticalMatchesList.Where(match => match.Count > 2).ToList();
            horizontalMatchesList = horizontalMatchesList.Where(match => match.Count > 2).ToList();
            
            _transitionContext.SetMatches(verticalMatchesList, horizontalMatchesList);
            Debug.Log(verticalMatchesList.Count);
            Debug.Log(horizontalMatchesList.Count);
            
            await ShakeFoundMatches(verticalMatchesList, horizontalMatchesList);

            return verticalMatchesList.Count > 0 || horizontalMatchesList.Count > 0;
        }

        private async UniTask ShakeFoundMatches(List<List<ArrayPositionData>> vertical, List<List<ArrayPositionData>> horizontal)
        {
            List<UniTask> shakeTasks = new List<UniTask>();
            
            foreach (var match in vertical)
            {
                foreach (var element in match)
                {
                    shakeTasks.Add(_gridView.MatchElements[element.RowIndex, element.ColumnIndex].Shake());
                }
            }

            foreach (var match in horizontal)
            {
                foreach (var element in match)
                {
                    shakeTasks.Add(_gridView.MatchElements[element.RowIndex, element.ColumnIndex].Shake());
                }
            }

            await UniTask.WhenAll(shakeTasks);
        }

        private bool IsElementWasIncludedInMatchBefore(List<List<ArrayPositionData>> matchesList, int i, int j)
        {
            return matchesList.Exists(match => match.Exists(element => element.RowIndex == i && element.ColumnIndex == j));
        }

        private List<ArrayPositionData> CheckMatchFigureHorizontal(int i, int j)
        {
            ElementType elementType = _gridView.MatchElements[i, j].ElementType;
            List<ArrayPositionData> matchedElements = new List<ArrayPositionData>();
            matchedElements.Add(new ArrayPositionData(i, j));
            // left
            if (j > 1)
            {
                for (int k = j - 1; k >= 0; k--)
                {
                    if (_gridView.MatchElements[i, k].ElementType != elementType)
                    {
                        break;
                    }
                    
                    matchedElements.Add(new ArrayPositionData(i, k));
                }
            }
            
            // right
            if (j < _gridView.MatchElements.GetLength(1) - 2)
            {
                for (int k = j + 1; k <= _gridView.MatchElements.GetLength(1) - 1; k++)
                {
                    if (_gridView.MatchElements[i, k].ElementType != elementType)
                    {
                        break;
                    }
                    
                    matchedElements.Add(new ArrayPositionData(i, k));
                }
            }
            
            return matchedElements;
        }

        private List<ArrayPositionData> CheckMatchFigureVertical(int i, int j)
        {
            ElementType elementType = _gridView.MatchElements[i, j].ElementType;
            List<ArrayPositionData> matchedElements = new List<ArrayPositionData>();
            matchedElements.Add(new ArrayPositionData(i, j));
            // up
            if (i > 1)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    if (_gridView.MatchElements[k, j].ElementType != elementType)
                    {
                        break;
                    }
                    
                    matchedElements.Add(new ArrayPositionData(k, j));
                }
            }
            
            // down
            if (i < _gridView.MatchElements.GetLength(0) - 2)
            {
                for (int k = i + 1; k <= _gridView.MatchElements.GetLength(0) - 1; k++)
                {
                    if (_gridView.MatchElements[k, j].ElementType != elementType)
                    {
                        break;
                    }
                    
                    matchedElements.Add(new ArrayPositionData(k, j));
                }
            }
            
            return matchedElements;
        }
    }
}