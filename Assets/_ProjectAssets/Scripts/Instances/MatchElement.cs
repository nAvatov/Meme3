using System;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _ProjectAssets.Scripts.Instances
{
    public class MatchElement : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        private ElementType _elementType;

        public ElementType ElementType => _elementType;

        public void SetElementType(ElementType type)
        {
            _elementType = type;
            _image.color = DecideColor();
        }

        public void Explode()
        {
            //_background.color = new Color(0, 0, 0, 1);
        }
        

        private Color DecideColor()
        {
            return _elementType switch
            {
                ElementType.Red => Color.red,
                ElementType.Blue => Color.blue,
                ElementType.Green => Color.green,
                ElementType.Yellow => Color.yellow,
                _ => Color.white
            };
        }

        public async UniTask Shake()
        {
            await gameObject.transform.DOShakeRotation(1f, new Vector3(0, 0, 25), 50, 90f, true)
                .OnComplete(() =>
                {
                    gameObject.transform.rotation = quaternion.identity;
                    gameObject.transform.DOKill();
                })
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}
