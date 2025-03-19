using System;
using System.Collections.Generic;
using _ProjectAssets.Scripts;
using _ProjectAssets.Scripts.Instances;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public enum ElementColor
{
    Red,
    Blue,
    Green,
    Yellow
}

public struct ElementDropTicket
{
    public MatchElement MatchElement;
    public ArrayPositionData ArrayPosition;
}

public struct ArrayPositionData
{
    public int RowIndex;
    public int ColumnIndex;

    public ArrayPositionData(int rowIndex, int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }
}

public class GridView : MonoBehaviour
{
    [SerializeField] private int _fieldColumns;
    
    [SerializeField] private List<GameObject> _spawnSpots;
    [SerializeField] private List<GameObject> _targetSpots;
    [SerializeField] private MatchElement _matchElementPrefab;

    [Header("Drop animation properties")] 
    [SerializeField] private int _dropDurationLowerBound;
    [SerializeField] private int _dropDurationUpperBound;
    
    private MatchElement[,] _matchElements;
    private GameObject[,] _targetSpotsArray;
    private GameObject[,] _spawnSpotsArray;

    private Random _dropAnimRnd;

    public int ColumnsAmount => _fieldColumns;
    public int RowsAmount => _spawnSpots.Count / _fieldColumns;
    
    
    public List<GameObject> TargetSpots => _targetSpots;
    public MatchElement[,] MatchElements => _matchElements;

    public void Spawn(ElementColor[,] elementsMatrix)
    {
        if (elementsMatrix != null)
        {
            int k = 0;
            _matchElements = new MatchElement[elementsMatrix.GetLength(0), elementsMatrix.GetLength(1)];
            
            for (int i = 0; i < elementsMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < elementsMatrix.GetLength(1); j++)
                {
                    _matchElements[i, j] = Instantiate(_matchElementPrefab, _spawnSpots[k].transform);
                    _matchElements[i, j].SetColor(elementsMatrix[i, j]);
                    k++;
                }
            }
        }
        
        _targetSpotsArray = CollectionWrapper.WrapListToTwoDimArray(_targetSpots, _fieldColumns);
        _dropAnimRnd = new Random();
    }

    public void AnimateDrop(List<ElementDropTicket> rowOfShuffledElements)
    {
        float resultingDuration = 0f;
        foreach (var element in rowOfShuffledElements)
        {
            
            element.MatchElement.transform.SetParent(_targetSpotsArray[element.ArrayPosition.RowIndex, element.ArrayPosition.ColumnIndex].transform);
            
            resultingDuration = _dropAnimRnd.Next(_dropDurationLowerBound, _dropDurationUpperBound) / 10f;
            element.MatchElement.transform.DOLocalMove(Vector3.zero, resultingDuration).SetEase(Ease.InCubic);
        }
    }

    public void Swap()
    {
        
    }
}
