using System.Collections.Generic;
using System.Linq;
using _ProjectAssets.Scripts.FSM;
using _ProjectAssets.Scripts.Instances;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.Game_States
{
    public class CheckMatchesState : GameState
    {

        private GridView _gridView;
        private FSMachine _fsm;

        [Inject]
        public void Construct(GridView gridView, FSMachine fsm)
        {
            _gridView = gridView;
            _fsm = fsm;
        }

        public override void Enter()
        {
            if (IsMatchFound())
            {
                Debug.Log("Matches found!");
                _fsm.ChangeState<DestroyMatchedElementsState>();
            }
            else
            {
                _fsm.ChangeState<MoveMatchesState>();
            }
        }

        public override void Exit()
        {

        }

        public override void SetNextState()
        {

        }

        private bool IsMatchFound()
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
            DebugMatchCheck(verticalMatchesList, horizontalMatchesList);

            return verticalMatchesList.Count > 0 && horizontalMatchesList.Count > 0;
        }

        private void DebugMatchCheck(List<List<ArrayPositionData>> vertical, List<List<ArrayPositionData>> horizontal)
        {
            foreach (var match in vertical)
            {
                if (match.Count > 2)
                {
                    foreach (var element in match)
                    {
                        _gridView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                    }
                }
            }
            
            foreach (var match in horizontal)
            {
                if (match.Count > 2)
                {
                    foreach (var element in match)
                    {
                        _gridView.MatchElements[element.RowIndex, element.ColumnIndex].Explode();
                    }
                }
            }
        }

        private bool IsElementWasIncludedInMatchBefore(List<List<ArrayPositionData>> matchesList, int i, int j)
        {
            return matchesList.Exists(match => match.Exists(element => element.RowIndex == i && element.ColumnIndex == j));
        }

        private List<ArrayPositionData> CheckMatchFigureHorizontal(int i, int j)
        {
            ElementColor elementColor = _gridView.MatchElements[i, j].ElementType;
            List<ArrayPositionData> matchedElements = new List<ArrayPositionData>();
            matchedElements.Add(new ArrayPositionData(i, j));
            // left
            if (j > 1)
            {
                for (int k = j - 1; k >= 0; k--)
                {
                    if (_gridView.MatchElements[i, k].ElementType != elementColor)
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
                    if (_gridView.MatchElements[i, k].ElementType != elementColor)
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
            ElementColor elementColor = _gridView.MatchElements[i, j].ElementType;
            List<ArrayPositionData> matchedElements = new List<ArrayPositionData>();
            matchedElements.Add(new ArrayPositionData(i, j));
            // up
            if (i > 1)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    if (_gridView.MatchElements[k, j].ElementType != elementColor)
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
                    if (_gridView.MatchElements[k, j].ElementType != elementColor)
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