using System;
using System.Collections.Generic;
using _ProjectAssets.Scripts.Installers;
using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class PrimaryGenerationState : GameState
    {
        private GameFieldView _gameFieldView;
        private FSMachine _fsm;
        private System.Random _generationRnd;
        private PlayabilityCheckModule _playabilityCheckModule;
        
        [Inject]
        public void Construct(GameFieldView gameFieldView, FSMachine fsm, System.Random generationRnd, PlayabilityCheckModule playabilityCheckModule)
        {
            _gameFieldView = gameFieldView;
            _fsm = fsm;
            _generationRnd = generationRnd;
            _playabilityCheckModule = playabilityCheckModule;
        }
        
        public override async void Enter()
        {
            _gameFieldView.HandleFieldInteractability(false);
            _gameFieldView.Spawn();
            
            GenerateElements();
            await CalculateDropOrder();
            
            _fsm.ChangeState<CheckMatchesState>();
        }

        private void GenerateElements()
        {
            ElementType[,] generation = new ElementType[_gameFieldView.RowsAmount, _gameFieldView.ColumnsAmount];
            int upperRandomBound = Enum.GetValues(typeof(ElementType)).Length;

            do
            {
                for (int i = 0; i < generation.GetLength(0); i++)
                {
                    for (int j = 0; j < generation.GetLength(1); j++)
                    {
                        generation[i, j] = new ElementType();
                        generation[i, j] = (ElementType)_generationRnd.Next(0, upperRandomBound);
                    }
                }

                _gameFieldView.SetTypeGeneration(generation);
                
            } while (!_playabilityCheckModule.IsGameFieldPlayable(_gameFieldView.MatchElements));
        }

        private bool CheckGenerationValidity()
        {
            return true;
        }

        private async UniTask CalculateDropOrder()
        {
            if (_gameFieldView.ColumnsAmount != _gameFieldView.MatchElements.GetLength(1))
            {
                Debug.Log("Items and spots dimensions are unmatched. Expected columns : " + _gameFieldView.ColumnsAmount+ ", got " + _gameFieldView.MatchElements.GetUpperBound(1));
                return;
            }
        
            int randomColumnIndex = 0;
            ElementDropTicket currentDropTicket;
        
            List<ElementDropTicket> rowOfShuffledElements = new List<ElementDropTicket>();
            _generationRnd = new System.Random();

            for (int i = _gameFieldView.MatchElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < _gameFieldView.MatchElements.GetLength(1); j++)
                {
                    randomColumnIndex = _generationRnd.Next(0, _gameFieldView.ColumnsAmount);
                    // TODO: Update target collection of random pick should be more effective way
                    while (rowOfShuffledElements.Exists(x => x.MatchElement == _gameFieldView.MatchElements[i, randomColumnIndex]))
                    {
                        randomColumnIndex = _generationRnd.Next(0, _gameFieldView.ColumnsAmount);
                    }
                    
                    currentDropTicket = new ElementDropTicket();
                    currentDropTicket.ArrayPosition = new ArrayPositionData();
                    currentDropTicket.ArrayPosition.ColumnIndex = randomColumnIndex;
                    currentDropTicket.ArrayPosition.RowIndex = i;
                    currentDropTicket.MatchElement = _gameFieldView.MatchElements[i, randomColumnIndex];
                    rowOfShuffledElements.Add(currentDropTicket);
                }
                
                await _gameFieldView.AnimateInitialDrop(rowOfShuffledElements);
                rowOfShuffledElements.Clear();
            }
        }
    }
}