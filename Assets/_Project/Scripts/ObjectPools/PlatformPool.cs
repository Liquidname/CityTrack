using _Project.Scripts.Map;
using UnityEngine;

namespace _Project.Scripts.ObjectPools
{
    public class PlatformPool : ObjectPool<Transform>
    {
        public PlatformPool(MapContextView context)
            : base(context.PlatformPrefabs, context.PlatformRoot)
        {
            
        }
            
    }
}