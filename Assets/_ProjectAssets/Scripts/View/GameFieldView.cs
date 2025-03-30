using System.Collections.Generic;
using _ProjectAssets.Scripts.Installers;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace _ProjectAssets.Scripts.View
{
    public class GameFieldView : MonoBehaviour
    {
        [SerializeField] private int _fieldColumns;
    
        [SerializeField] private List<GameObject> _spawnSpots;
        [SerializeField] private List<GameObject> _targetSpots;
        [SerializeField] private MatchElement _matchElementPrefab;

        [Header("Drop animation properties")] 
        [SerializeField] private int _dropDurationLowerBound;
        [SerializeField] private int _dropDurationUpperBound;

        [Header("Move elements properties")] 
        [SerializeField] private CanvasGroup _fieldCanvasGroup;
        [SerializeField] private GameObject _swapLayer;
    
        private MatchElement[,] _matchElements;
        private MatchElement[,] _reservedElements;
    

        private Random _dropAnimRnd;
        private SignalBus _signalBus;

        public int ColumnsAmount => _fieldColumns;
        public int RowsAmount => _spawnSpots.Count / _fieldColumns;

        public GameObject[,] TargetSpotsArray { get; set; }
        public GameObject[,] SpawnSpotsArray { get; set; }
        public List<GameObject> TargetSpots => _targetSpots;
        public MatchElement[,] MatchElements => _matchElements;
        public MatchElement[,] ReservedElements => _reservedElements;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            SpawnSpotsArray = CollectionWrapper.WrapListToTwoDimArray(_spawnSpots, _fieldColumns);
        }

        public void Spawn()
        {
            int k = 0;
            _matchElements = new MatchElement[SpawnSpotsArray.GetLength(0), SpawnSpotsArray.GetLength(1)];
            _reservedElements = new MatchElement[SpawnSpotsArray.GetLength(0), SpawnSpotsArray.GetLength(1)];
            
            for (int i = 0; i < SpawnSpotsArray.GetLength(0); i++)
            {
                for (int j = 0; j < SpawnSpotsArray.GetLength(1); j++)
                {
                    _matchElements[i, j] = Instantiate(_matchElementPrefab, _spawnSpots[k].transform);
                    _matchElements[i, j].SetPositionData(new ArrayPositionData(i, j));
                    _matchElements[i, j].GetComponent<DragAndDrop>().Initialize(_swapLayer, _signalBus);
                    k++;
                }
            }
        
            TargetSpotsArray = CollectionWrapper.WrapListToTwoDimArray(_targetSpots, _fieldColumns);
            _dropAnimRnd = new Random();
        }

        public void SetTypeGeneration(ElementType[,] elementsMatrix)
        {
            if (elementsMatrix.Length == _matchElements.Length)
            {
                for (int i = 0; i < _matchElements.GetLength(0); i++)
                {
                    for (int j = 0; j < _matchElements.GetLength(1); j++)
                    {
                        _matchElements[i, j].SetElementType(elementsMatrix[i, j]);
                    }
                }   
            }
        }

        public async UniTask AnimateInitialDrop(List<ElementDropTicket> rowOfShuffledElements)
        {
        
            List<UniTask> animateTasks = new List<UniTask>();
            foreach (var element in rowOfShuffledElements)
            {
            
                element.MatchElement.transform.SetParent(TargetSpotsArray[element.ArrayPosition.RowIndex, element.ArrayPosition.ColumnIndex].transform);

                animateTasks.Add(AnimateElementMoveToIdentity(element.MatchElement));
            }
        
            await UniTask.WhenAll(animateTasks);
        }

        public void ChangeElementPositionBeforeDrop(ArrayPositionData initialPosition, int targetRowIndex, bool isInnerFieldDrop = true)
        {
            if (isInnerFieldDrop)
            {
                _matchElements[initialPosition.RowIndex, initialPosition.ColumnIndex].transform.SetParent(TargetSpotsArray[targetRowIndex, initialPosition.ColumnIndex].transform);
            }
            else
            {
                _reservedElements[initialPosition.RowIndex, initialPosition.ColumnIndex].transform.SetParent(TargetSpotsArray[targetRowIndex, initialPosition.ColumnIndex].transform);
            }
        }

        public async UniTask AnimateSingleElementDrop(ArrayPositionData initialPosition, int targetRowIndex)
        {
            await AnimateElementMoveToIdentity(_matchElements[targetRowIndex, initialPosition.ColumnIndex]);
            
            _matchElements[targetRowIndex, initialPosition.ColumnIndex].SetPositionData(new ArrayPositionData(targetRowIndex, initialPosition.ColumnIndex));
        }

        public void ReturnElementToSpawnPoint(int i, int j)
        {
            _matchElements[i, j].transform.SetParent(SpawnSpotsArray[i, j].transform);
            _matchElements[i, j].transform.localPosition = Vector3.zero;
        }

        private async UniTask AnimateElementMoveToIdentity(MatchElement element)
        {
            element.transform.DOComplete();
            await element.transform.DOLocalMove(Vector3.zero, _dropAnimRnd.Next(_dropDurationLowerBound, _dropDurationUpperBound) / 10f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    element.transform.localPosition = Vector3.zero;
                    element.transform.DOKill();
                })
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public void HandleFieldInteractability(bool isFieldInteractable)
        {
            _fieldCanvasGroup.interactable = isFieldInteractable;
            _fieldCanvasGroup.blocksRaycasts = isFieldInteractable;
        }

        public async UniTask Swap(ArrayPositionData el1, ArrayPositionData el2)
        {
            _matchElements[el1.RowIndex, el1.ColumnIndex].transform.SetParent(TargetSpotsArray[el2.RowIndex, el2.ColumnIndex].transform);
            _matchElements[el2.RowIndex, el2.ColumnIndex].transform.SetParent(TargetSpotsArray[el1.RowIndex, el1.ColumnIndex].transform);

            await AnimateElementMoveToIdentity(_matchElements[el1.RowIndex, el1.ColumnIndex]);
            await AnimateElementMoveToIdentity(_matchElements[el2.RowIndex, el2.ColumnIndex]);
        }

        public void ChangeInteractabilityOfSwapElements(ArrayPositionData el1, ArrayPositionData el2, bool isInteractable)
        {
            _matchElements[el1.RowIndex, el1.ColumnIndex].GetComponent<DragAndDrop>().ChangeInteractability(isInteractable);
            _matchElements[el2.RowIndex, el2.ColumnIndex].GetComponent<DragAndDrop>().ChangeInteractability(isInteractable);
        }
    }
}