using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace _ProjectAssets.Scripts.View
{
    public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        private GameObject _swapLayer;
        private GameObject _initialParent;
        private GameObject _dropTarget;
        private SignalBus _signalBus;
    
        public void Initialize(GameObject swapLayer, SignalBus signalBus)
        {
            _swapLayer = swapLayer;
            _signalBus = signalBus;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            _initialParent = transform.parent.gameObject;
            gameObject.transform.SetParent(_swapLayer.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            gameObject.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dropTarget = eventData.pointerCurrentRaycast.gameObject;

            if (_dropTarget && _dropTarget != gameObject)
            {
                if (_dropTarget.TryGetComponent<MatchElement>(out MatchElement targetMatchElement))
                {
                    _signalBus.Fire(
                        new SwapSignal
                        {
                            MovingElement = gameObject.GetComponent<MatchElement>().PositionData,
                            TargetElement = targetMatchElement.PositionData
                        });

                    return;
                }
            }
            
            _canvasGroup.blocksRaycasts = true;
            ReturnElementToInitialPosition();
        }

        public void ReturnElementToInitialPosition()
        {
            gameObject.transform.SetParent(_initialParent.transform);
            gameObject.transform.localPosition = Vector3.zero;
        }

        public void ChangeInteractability(bool isInteractible)
        {
            _canvasGroup.interactable = isInteractible;
            _canvasGroup.blocksRaycasts = isInteractible;
        } 
    }
}
