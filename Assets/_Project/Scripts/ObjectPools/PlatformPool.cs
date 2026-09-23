using _Project.Scripts.Map;
using UnityEngine;

namespace _Project.Scripts.ObjectPools
{
    public class PlatformPool : ObjectPool<PlatformView>
    {
        public PlatformPool(MapContextView context)
            : base(context.PlatformPrefabs, context.PlatformRoot)
        {
            
        }
            
    }
}