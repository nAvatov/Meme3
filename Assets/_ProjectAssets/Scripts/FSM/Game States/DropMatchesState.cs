using System.Collections.Generic;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class DropMatchesState : GameState
    {
        private FSMachine _fsm;
        private GameFieldView _gameFieldView;
        [Inject]
        public void Construct(FSMachine fsm, GameFieldView gameFieldView)
        {
            _fsm = fsm;
            _gameFieldView = gameFieldView;
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
            for (int i = _gameFieldView.MatchElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = _gameFieldView.MatchElements.GetLength(1) - 1; j >= 0; j--)
                {
                    if (!_gameFieldView.MatchElements[i, j])
                    {
                        continue;
                    }

                    dropTargetRow = FindLowestEmptySlot(i, j);

                    if (dropTargetRow != i)
                    {
                        currentElementPosData.ColumnIndex = j;
                        currentElementPosData.RowIndex = i;
                        
                        _gameFieldView.ChangeElementPositionBeforeDrop(currentElementPosData, dropTargetRow);
                        
                        _gameFieldView.MatchElements[dropTargetRow, currentElementPosData.ColumnIndex] = _gameFieldView.MatchElements[currentElementPosData.RowIndex, currentElementPosData.ColumnIndex];
                        _gameFieldView.MatchElements[currentElementPosData.RowIndex, currentElementPosData.ColumnIndex] = null;
                        
                        dropTasks.Add( _gameFieldView.AnimateSingleElementDrop(currentElementPosData, dropTargetRow));
                    }
                }
            }
            
            await UniTask.WhenAll(dropTasks);
        }

        private async UniTask DropNewElementsToEmptyPositions()
        {
            int dropTargetRow = 0;
            ArrayPositionData currentElementPosData = new ArrayPositionData();
            List<UniTask> dropTasks = new List<UniTask>();
            
            for (int i = _gameFieldView.ReservedElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = _gameFieldView.ReservedElements.GetLength(1) - 1; j >= 0; j--)
                {
                    if (!_gameFieldView.ReservedElements[i, j])
                    {
                        continue;
                    }
                    
                    dropTargetRow = FindLowestEmptySlot(0, j);

                    currentElementPosData.RowIndex = i;
                    currentElementPosData.ColumnIndex = j;
                    
                    _gameFieldView.ChangeElementPositionBeforeDrop(currentElementPosData, dropTargetRow, false);
                    
                    _gameFieldView.MatchElements[dropTargetRow, currentElementPosData.ColumnIndex] = _gameFieldView.ReservedElements[currentElementPosData.RowIndex, currentElementPosData.ColumnIndex];
                    _gameFieldView.ReservedElements[currentElementPosData.RowIndex, currentElementPosData.ColumnIndex] = null;
                    
                    dropTasks.Add(_gameFieldView.AnimateSingleElementDrop(currentElementPosData, dropTargetRow));
                }
            }
            
            await UniTask.WhenAll(dropTasks);
        }

        private int FindLowestEmptySlot(int initialElementRow, int column)
        {
            for (int i = _gameFieldView.MatchElements.GetLength(0) - 1; i > initialElementRow; i--)
            {
                if (!_gameFieldView.MatchElements[i, column])
                {
                    return i;
                }
            }

            return initialElementRow;
        }
    }
}