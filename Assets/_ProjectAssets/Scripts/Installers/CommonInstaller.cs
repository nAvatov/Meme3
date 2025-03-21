using _ProjectAssets.Scripts.View;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class CommonInstaller : MonoInstaller
    {
        [FormerlySerializedAs("_gridView")] [SerializeField] private GameFieldView _gameFieldView;
        public override void InstallBindings()
        {
            Container.BindInstance(_gameFieldView).AsSingle();
        }
    }
}