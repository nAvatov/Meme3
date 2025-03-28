using System.Collections.Generic;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using UnityEditorInternal;
using Zenject;

namespace _ProjectAssets.Scripts
{
    public class PlayabilityCheckModule
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
                    return IsSequenceGapRepairPossible(i, j) || IsHorizontalSequencePossible(i, j) || IsVerticalSequencePossible(i, j);
                }
            }

            return false;
        }

        private bool IsSequenceGapRepairPossible(int i, int j)
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
        
        // Potential match stack can be used as a hint for particular game mode
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

        private bool IsHorizontalSequencePossible(int i, int j, bool isDefaultArrayReadingStrategy = true)
        {
            ElementType targetElementType = _gameFieldView.MatchElements[i, j].ElementType;
            
            if (j > 0 && j < _gameFieldView.MatchElements.GetLength(1) - 1)
            {
                if (isDefaultArrayReadingStrategy)
                {
                    if (_gameFieldView.MatchElements[i, j + 1].ElementType == targetElementType)
                    {
                        if (j + 2 < _gameFieldView.MatchElements.GetLength(1) - 1)
                        {
                            if (_gameFieldView.MatchElements[i, j + 3].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (i > 0)
                        {
                            if (_gameFieldView.MatchElements[i - 1, j + 2].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (i < _gameFieldView.MatchElements.GetLength(0) - 1)
                        {
                            if (_gameFieldView.MatchElements[i + 1, j + 2].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }
                    }   
                }
                else
                {
                    if (_gameFieldView.MatchElements[i, j - 1].ElementType == targetElementType)
                    {
                        if (j - 2 > 0)
                        {
                            if (_gameFieldView.MatchElements[i, j - 3].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (i > 0)
                        {
                            if (_gameFieldView.MatchElements[i - 1, j - 2].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (i < _gameFieldView.MatchElements.GetLength(0) - 1)
                        {
                            if (_gameFieldView.MatchElements[i + 1, j - 2].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        
        private bool IsVerticalSequencePossible(int i, int j, bool isDefaultArrayReadingStrategy = true)
        {
            ElementType targetElementType = _gameFieldView.MatchElements[i, j].ElementType;

            if (i > 0 && i < _gameFieldView.MatchElements.GetLength(0) - 1)
            {
                if (isDefaultArrayReadingStrategy)
                {
                    if (_gameFieldView.MatchElements[i + 1, j].ElementType == targetElementType && i + 1 < _gameFieldView.MatchElements.GetLength(0) - 1)
                    {
                        if (i + 2 < _gameFieldView.MatchElements.GetLength(0) - 1)
                        {
                            if (_gameFieldView.MatchElements[i + 3, j].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (j > 0)
                        {
                            if (_gameFieldView.MatchElements[i + 2, j - 1].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (j < _gameFieldView.MatchElements.GetLength(1) - 1)
                        {
                            if (_gameFieldView.MatchElements[i + 2, j + 1].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (_gameFieldView.MatchElements[i - 1, j].ElementType == targetElementType && i - 1 > 0)
                    {
                        if (i - 2 < _gameFieldView.MatchElements.GetLength(0) - 1)
                        {
                            if (_gameFieldView.MatchElements[i - 3, j].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (j > 0)
                        {
                            if (_gameFieldView.MatchElements[i - 2, j - 1].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }

                        if (j < _gameFieldView.MatchElements.GetLength(1) - 1)
                        {
                            if (_gameFieldView.MatchElements[i - 2, j + 1].ElementType == targetElementType)
                            {
                                return true;
                            }
                        }
                    }
                }
                
            }

            return false;
        }
    }
}