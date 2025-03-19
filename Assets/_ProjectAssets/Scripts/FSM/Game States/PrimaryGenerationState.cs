using System;
using System.Collections.Generic;
using _ProjectAssets.Scripts.Structures;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.FSM.Game_States
{
    public class PrimaryGenerationState : GameState
    {
        private GridView _gridView;
        private FSMachine _fsm;
        private System.Random _generationRnd;
        
        [Inject]
        public void Construct(GridView gridView, FSMachine fsm)
        {
            _gridView = gridView;
            _fsm = fsm;
        }
        
        public override async void Enter()
        {
            GenerateElements();
            await CalculateDropOrder();
            SetNextState();
        }

        public override void SetNextState()
        {
            _fsm.ChangeState<CheckMatchesState>();
        }


        private void GenerateElements()
        {
            ElementColor[,] generation = new ElementColor[_gridView.RowsAmount, _gridView.ColumnsAmount];
            _generationRnd = new System.Random();
            int upperRandomBound = Enum.GetValues(typeof(ElementColor)).Length;

            do
            {
                for (int i = 0; i < generation.GetLength(0); i++)
                {
                    for (int j = 0; j < generation.GetLength(1); j++)
                    {
                        generation[i, j] = new ElementColor();
                        generation[i, j] = (ElementColor)_generationRnd.Next(0, upperRandomBound);
                    }
                }
            } while (!CheckGenerationValidity());
            
            _gridView.Spawn(generation);
        }

        private bool CheckGenerationValidity()
        {
            return true;
        }

        private async UniTask CalculateDropOrder()
        {
            if (_gridView.ColumnsAmount != _gridView.MatchElements.GetLength(1))
            {
                Debug.Log("Items and spots dimensions are unmatched. Expected columns : " + _gridView.ColumnsAmount+ ", got " + _gridView.MatchElements.GetUpperBound(1));
                return;
            }
        
            int randomColumnIndex = 0;
            ElementDropTicket currentDropTicket;
        
            List<ElementDropTicket> rowOfShuffledElements = new List<ElementDropTicket>();
            _generationRnd = new System.Random();

            for (int i = _gridView.MatchElements.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < _gridView.MatchElements.GetLength(1); j++)
                {
                    randomColumnIndex = _generationRnd.Next(0, _gridView.ColumnsAmount);
                    // TODO: Update target collection of random pick should be more effective way
                    while (rowOfShuffledElements.Exists(x => x.MatchElement == _gridView.MatchElements[i, randomColumnIndex]))
                    {
                        randomColumnIndex = _generationRnd.Next(0, _gridView.ColumnsAmount);
                    }
                    
                    currentDropTicket = new ElementDropTicket();
                    currentDropTicket.ArrayPosition = new ArrayPositionData();
                    currentDropTicket.ArrayPosition.ColumnIndex = randomColumnIndex;
                    currentDropTicket.ArrayPosition.RowIndex = i;
                    currentDropTicket.MatchElement = _gridView.MatchElements[i, randomColumnIndex];
                    rowOfShuffledElements.Add(currentDropTicket);
                }
                
                await _gridView.AnimateDrop(rowOfShuffledElements);
                rowOfShuffledElements.Clear();
            }
        }
    }
}