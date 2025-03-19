using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _ProjectAssets.Scripts.Instances
{
    public class MatchElement : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private ElementColor _color;

        public void SetColor(ElementColor color)
        {
            _color = color;
            _image.color = DecideColor();
        }

        public void Explode()
        {
        
        }

        private Color DecideColor()
        {
            return _color switch
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
