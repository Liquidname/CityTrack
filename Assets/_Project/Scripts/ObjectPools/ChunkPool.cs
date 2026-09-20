using _Project.Scripts.Map;

namespace _Project.Scripts.ObjectPools
{
    public class ChunkPool : ObjectPool<ChunkView>
    {
        public ChunkPool(MapContextView context) 
            : base(context.СhunkPrefabs, context.PoolRoot)
        {
        }
    }
}