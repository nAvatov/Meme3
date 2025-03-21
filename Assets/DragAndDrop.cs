using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject _swapLayer;
    private GameObject _initialParent;
    
    public void Initialize(GameObject swapLayer)
    {
        _swapLayer = swapLayer;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialParent = transform.parent.gameObject;
        gameObject.transform.SetParent(_swapLayer.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(Input.mousePosition.normalized);
        gameObject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(_initialParent.transform);
        gameObject.transform.localPosition = Vector3.zero;
    }
}
