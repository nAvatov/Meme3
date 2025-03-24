using System;
using System.Collections;
using System.Collections.Generic;
using _ProjectAssets.Scripts.Installers;
using _ProjectAssets.Scripts.Instances;
using _ProjectAssets.Scripts.Structures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Zenject;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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
        _initialParent = transform.parent.gameObject;
        gameObject.transform.SetParent(_swapLayer.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(_initialParent.transform);
        gameObject.transform.localPosition = Vector3.zero;
        
        _dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (_dropTarget && _dropTarget != gameObject)
        {
            Debug.Log(_signalBus);
            Debug.Log(gameObject.GetComponent<MatchElement>().PositionData);
            Debug.Log(_dropTarget.name);
            Debug.Log(_dropTarget.GetComponent<MatchElement>().PositionData);
            
            _signalBus.Fire(
                new SwapSignal
                {
                    MovingElement = gameObject.GetComponent<MatchElement>().PositionData,
                    TargetElement = _dropTarget.GetComponent<MatchElement>().PositionData
                });
        }
    }
}
