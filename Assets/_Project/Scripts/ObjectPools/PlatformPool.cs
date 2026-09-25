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
            _roofsPool = new ObjectPool<PlatformView>(context.PlatformPrefabs, context.PlatformRoofsRoot);
            _buildingsPool = new ObjectPool<PlatformView>(context.PlatformPrefabs, context.PlatformBuildingRoot);
        }

        public PlatformView Get(MapLayer layer, Vector3 position, Transform parent)
        {
            if (layer == MapLayer.ROOFS)
            {
                return _roofsPool.Get(position, parent);
            } 
            
            if (layer == MapLayer.BUILDING)
            {
                return _buildingsPool.Get(position, parent);
            }
            
            return null;
        }

        public void Return(PlatformView platformView)
        {
            if (platformView.SpawnLayer == MapLayer.ROOFS)
            {
                _roofsPool.Return(platformView);
            }
            else if(platformView.SpawnLayer == MapLayer.BUILDING)
            {
                _buildingsPool.Return(platformView);
            }
        }
    }
}