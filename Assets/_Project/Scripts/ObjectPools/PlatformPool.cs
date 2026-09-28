using _Project.Scripts.Map;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.ObjectPools
{
    public class PlatformPool
    {
        private readonly ObjectPool<PlatformView> _roofsPool;
        private readonly ObjectPool<PlatformView> _buildingsPool;
        public PlatformPool(MapContextView context)
        {
            _roofsPool = new ObjectPool<PlatformView>(context.PlatformRoofsPrefabs, context.PlatformRoofsRoot);
            _buildingsPool = new ObjectPool<PlatformView>(context.PlatformBuildingPrefabs, context.PlatformBuildingRoot);
        }

        public PlatformView Get(MapLayer layer, Vector3 position, Transform parent)
        {
            PlatformView platform = layer switch
            {
                MapLayer.BUILDING => _buildingsPool.Get(position, parent),
                MapLayer.ROOFS => _roofsPool.Get(position, parent),
                _ => null
            };
            
            platform?.SetLayer(layer);
            return platform;
        }

        public void Return(PlatformView platformView)
        {
            switch (platformView.CurrentLayer)
            {
                case MapLayer.BUILDING:
                    _buildingsPool.Return(platformView);
                    break;
                case MapLayer.ROOFS:
                    _roofsPool.Return(platformView);
                    break;
                default:
                    Debug.LogError("Unknown layer " + platformView.CurrentLayer);
                    break;
            }
        }
    }
}