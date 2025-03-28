using System.Collections.Generic;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.View;
using UnityEditorInternal;
using Zenject;

namespace _ProjectAssets.Scripts
{
    public class MatchPlayabilityModule
    {
        private GameFieldView _gameFieldView;
        List<MatchElement> _circledMatchElements = new List<MatchElement>(4);
        
        [Inject]
        public void Construct(GameFieldView gameFieldView)
        {
            _gameFieldView = gameFieldView;
        }

        public bool IsGameFieldPlayable()
        {
            for (int i = 0; i < _gameFieldView.MatchElements.GetLength(0); i++)
            {
                for (int j = 0; j < _gameFieldView.MatchElements.GetLength(1); j++)
                {
                    return IsGapRepairAvailable(i, j) || IsThirdInARowAvailable(i, j);
                }
            }

            return false;
        }

        private bool IsGapRepairAvailable(int i, int j)
        {
            if ((i == 0 || i == _gameFieldView.MatchElements.GetLength(0) - 1) && (j == 0 || j == _gameFieldView.MatchElements.GetLength(1) - 1)) 
            {
                return false;
            }
            
            if (j > 0 && i > 0 && i < _gameFieldView.MatchElements.GetLength(0) - 1 && j < _gameFieldView.MatchElements.GetLength(1) - 1)
            {
                _circledMatchElements.Add(_gameFieldView.MatchElements[i, j + 1]);
                _circledMatchElements.Add(_gameFieldView.MatchElements[i, j - 1]);
                _circledMatchElements.Add(_gameFieldView.MatchElements[i + 1, j]);
                _circledMatchElements.Add(_gameFieldView.MatchElements[i - 1, j]);

                return IsAtleastThreeMatchesSame();
            }
            
            return IsBorderGapAvailable(i, j);
        }

        private bool IsBorderGapAvailable(int i, int j)
        {
            if (j == 0)
            {
                return _gameFieldView.MatchElements[i, j + 1].ElementType == _gameFieldView.MatchElements[i + 1, j].ElementType && _gameFieldView.MatchElements[i, j + 1].ElementType == _gameFieldView.MatchElements[i - 1, j].ElementType;
            }

            if (j == _gameFieldView.MatchElements.GetLength(0) - 1)
            {
                return _gameFieldView.MatchElements[i, j - 1].ElementType == _gameFieldView.MatchElements[i + 1, j].ElementType && _gameFieldView.MatchElements[i, j - 1].ElementType == _gameFieldView.MatchElements[i - 1, j].ElementType;
            }

            if (i == 0)
            {
                return _gameFieldView.MatchElements[i + 1, j].ElementType == _gameFieldView.MatchElements[i, j - 1].ElementType && _gameFieldView.MatchElements[i + 1, j].ElementType == _gameFieldView.MatchElements[i, j + 1].ElementType;
            }

            if (i == _gameFieldView.MatchElements.GetLength(0) - 1)
            {
                return  _gameFieldView.MatchElements[i - 1, j].ElementType == _gameFieldView.MatchElements[i, j - 1].ElementType && _gameFieldView.MatchElements[i - 1, j].ElementType == _gameFieldView.MatchElements[i, j + 1].ElementType;
            }

            return false;
        }

        private bool IsAtleastThreeMatchesSame()
        {
            int sameTypeMatchesAmount = 0;
            ElementType currentType;

            for (int i = 0; i < _circledMatchElements.Count; i++)
            {
                currentType = _circledMatchElements[i].ElementType;
                
                for (int j = 0; j < _circledMatchElements.Count; j++)
                {
                    if (_circledMatchElements[j].ElementType == currentType)
                    {
                        sameTypeMatchesAmount++;
                        
                        if (sameTypeMatchesAmount >= 3)
                        {
                            _circledMatchElements.Clear();
                            return true;
                        }
                    }    
                }

                sameTypeMatchesAmount = 0;
            }

            _circledMatchElements.Clear();
            return false;
        }

        private bool IsThirdInARowAvailable(int i, int j)
        {
            return false;
        }
    }
}