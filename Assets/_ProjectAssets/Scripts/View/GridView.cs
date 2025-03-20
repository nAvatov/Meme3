using System.Collections.Generic;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

namespace _ProjectAssets.Scripts.View
{
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
        private MatchElement[,] _reservedElements;
    

        private Random _dropAnimRnd;

        public int ColumnsAmount => _fieldColumns;
        public int RowsAmount => _spawnSpots.Count / _fieldColumns;

        public GameObject[,] TargetSpotsArray { get; set; }
        public GameObject[,] SpawnSpotsArray { get; set; }
    
        public List<GameObject> TargetSpots => _targetSpots;
        public MatchElement[,] MatchElements => _matchElements;
        public MatchElement[,] ReservedElements => _reservedElements;

        public void Spawn(ElementType[,] elementsMatrix)
        {
            if (elementsMatrix != null)
            {
                int k = 0;
                _matchElements = new MatchElement[elementsMatrix.GetLength(0), elementsMatrix.GetLength(1)];
                _reservedElements = new MatchElement[elementsMatrix.GetLength(0), elementsMatrix.GetLength(1)];
            
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
        
            TargetSpotsArray = CollectionWrapper.WrapListToTwoDimArray(_targetSpots, _fieldColumns);
            _dropAnimRnd = new Random();
        }

        public async UniTask AnimateInitialDrop(List<ElementDropTicket> rowOfShuffledElements)
        {
        
            List<UniTask> animateTasks = new List<UniTask>();
            foreach (var element in rowOfShuffledElements)
            {
            
                element.MatchElement.transform.SetParent(TargetSpotsArray[element.ArrayPosition.RowIndex, element.ArrayPosition.ColumnIndex].transform);

                animateTasks.Add(AnimateElementDrop(element.MatchElement));
            }
        
            await UniTask.WhenAll(animateTasks);
        }

        public async UniTask AnimateSingleElementDrop(ArrayPositionData initialPosition, int targetRowIndex, bool isInnerFieldDrop = true)
        {
            if (isInnerFieldDrop)
            {
                _matchElements[initialPosition.RowIndex, initialPosition.ColumnIndex].transform.SetParent(TargetSpotsArray[targetRowIndex, initialPosition.ColumnIndex].transform);
                _matchElements[targetRowIndex, initialPosition.ColumnIndex] = _matchElements[initialPosition.RowIndex, initialPosition.ColumnIndex];
                _matchElements[initialPosition.RowIndex, initialPosition.ColumnIndex] = null;
                
                await AnimateElementDrop(_matchElements[targetRowIndex, initialPosition.ColumnIndex], true);
            }
            else
            {
                _reservedElements[initialPosition.RowIndex, initialPosition.ColumnIndex].transform.SetParent(TargetSpotsArray[targetRowIndex, initialPosition.ColumnIndex].transform);
                _matchElements[targetRowIndex, initialPosition.ColumnIndex] = _reservedElements[initialPosition.RowIndex, initialPosition.ColumnIndex];
                _reservedElements[initialPosition.RowIndex, initialPosition.ColumnIndex] = null;
                
                await AnimateElementDrop(_matchElements[targetRowIndex, initialPosition.ColumnIndex], true);
            }
        }

        public void ReturnElementToSpawnPoint(int i, int j)
        {
            if (SpawnSpotsArray == null)
            {
                SpawnSpotsArray = CollectionWrapper.WrapListToTwoDimArray(_spawnSpots, _fieldColumns);
            }
        
            _matchElements[i, j].transform.SetParent(SpawnSpotsArray[i, j].transform);
            SpawnSpotsArray[i, j] = _matchElements[i, j].gameObject;
            SpawnSpotsArray[i, j].transform.localPosition = Vector3.zero;
            _reservedElements[i, j] = _matchElements[i, j];
            _matchElements[i, j] = null;
        }

        private async UniTask AnimateElementDrop(MatchElement element, bool slow = false)
        {
            await element.transform.DOLocalMove(Vector3.zero, _dropAnimRnd.Next(_dropDurationLowerBound, slow ? _dropDurationUpperBound + 10 : _dropDurationUpperBound) / 10f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public void Swap()
        {
        
        }
    }
}