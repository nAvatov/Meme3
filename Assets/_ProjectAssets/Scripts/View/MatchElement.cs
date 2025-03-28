using _ProjectAssets.Scripts.Structures;
using _ProjectAssets.Scripts.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectAssets.Scripts.Instances
{
    public class MatchElement : MonoBehaviour
    {
        [SerializeField] private Image _typeImage;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _posText;
        private ElementType _elementType;

        public ArrayPositionData PositionData { get; private set; }
        public ElementType ElementType => _elementType;

        public void SetElementType(ElementType type)
        {
            _elementType = type;
            _typeImage.color = DecideColor();
        }
        
        public void SetPositionData(ArrayPositionData arrayPositionData)
        {
            //_posText.SetText(arrayPositionData.RowIndex + " " + arrayPositionData.ColumnIndex);
            PositionData = arrayPositionData;
        }

        public async UniTask Shake()
        {
            await gameObject.transform.DOShakeRotation(1f, new Vector3(0, 0, 10), 50, 45f, false)
                .OnComplete(() =>
                {
                    gameObject.transform.rotation = quaternion.identity;
                    gameObject.transform.DOKill();
                })
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public void Explode()
        {
            
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

        private void OnDestroy()
        {
            gameObject.transform.DOKill();
        }
    }
}
