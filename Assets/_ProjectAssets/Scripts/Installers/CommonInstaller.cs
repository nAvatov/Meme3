using _ProjectAssets.Scripts.View;
using UnityEngine;
using Zenject;

namespace _ProjectAssets.Scripts.Installers
{
    public class CommonInstaller : MonoInstaller
    {
        [SerializeField] private GridView _gridView;
        public override void InstallBindings()
        {
            Container.BindInstance(_gridView).AsSingle();
        }
    }
}