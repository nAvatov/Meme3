using UnityEngine;
using UnityEngine.UI;

namespace _ProjectAssets.Scripts.Instances
{
    public class MatchElement : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        private ElementColor _elementType;

        public ElementColor ElementType => _elementType;

        public void SetColor(ElementColor color)
        {
            _elementType = color;
            _image.color = DecideColor();
        }

        public void Explode()
        {
            _background.color = new Color(0, 0, 0, 1);
        }

        private Color DecideColor()
        {
            return _elementType switch
            {
                ElementColor.Red => Color.red,
                ElementColor.Blue => Color.blue,
                ElementColor.Green => Color.green,
                ElementColor.Yellow => Color.yellow,
                _ => Color.white
            };
        }
    }
}
