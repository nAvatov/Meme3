using System.Collections.Generic;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class DropMatchesState : GameState
    {
        private FSMachine _fsm;
        private GridView _gridView;
        [Inject]
        public void Construct(FSMachine fsm, GridView gridView)
        {
            _fsm = fsm;
            _gridView = gridView;
        }
        public override async void Enter()
        {
            await DropElementsToNewPositions();
            await DropNewElementsToEmptyPositions();
            _fsm.ChangeState<CheckMatchesState>();
        }

        private async UniTask DropElementsToNewPositions()
        {
            int dropTargetRow = 0;
            ArrayPositionData currentElementPosData = new ArrayPositionData();
            List<UniTask> dropTasks = new List<UniTask>();
            for (int i = _gridView.MatchElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = _gridView.MatchElements.GetLength(1) - 1; j >= 0; j--)
                {
                    if (!_gridView.MatchElements[i, j])
                    {
                        continue;
                    }

                    dropTargetRow = FindLowestEmptySlot(i, j);

                    if (dropTargetRow != i)
                    {
                        currentElementPosData.ColumnIndex = j;
                        currentElementPosData.RowIndex = i;
                        await _gridView.AnimateSingleElementDrop(currentElementPosData, dropTargetRow);
                    }
                }
            }
            
            //await UniTask.WhenAll(dropTasks);
        }

        private async UniTask DropNewElementsToEmptyPositions()
        {
            int dropTargetRow = 0;
            ArrayPositionData currentElementPosData = new ArrayPositionData();
            List<UniTask> dropTasks = new List<UniTask>();
            
            for (int i = _gridView.ReservedElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = _gridView.ReservedElements.GetLength(1) - 1; j >= 0; j--)
                {
                    if (!_gridView.ReservedElements[i, j])
                    {
                        continue;
                    }
                    
                    dropTargetRow = FindLowestEmptySlot(0, j);

                    currentElementPosData.RowIndex = i;
                    currentElementPosData.ColumnIndex = j;
                    await _gridView.AnimateSingleElementDrop(currentElementPosData, dropTargetRow, false);
                }
            }
            
            await UniTask.WhenAll(dropTasks);
        }

        private int FindLowestEmptySlot(int initialElementRow, int column)
        {
            for (int i = _gridView.MatchElements.GetLength(0) - 1; i > initialElementRow; i--)
            {
                if (_gridView.MatchElements[i, column] == null)
                {
                    return i;
                }
            }

            return initialElementRow;
        }
    }
}