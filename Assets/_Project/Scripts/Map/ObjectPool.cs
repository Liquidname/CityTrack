using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Map
{
    public class ObjectPool
    {
        private readonly ChunkView[] _chunkPrefabs;
        private readonly Transform _poolRoot;
        private readonly Queue<ChunkView> _pooledObjects = new Queue<ChunkView>();
        
        private Transform _chunkParent;
        
        private int initialCapacity = 5;
        
        public ObjectPool(MapContextView mapContextView)
        {
            _chunkPrefabs = mapContextView.СhunkPrefabs;
            _poolRoot = mapContextView.PoolRoot;
            _chunkParent = mapContextView.ChunkParent;
            
            for (int i = 0; i < _chunkPrefabs.Length; i++)
            {
                for (int j = 0; j < initialCapacity; j++)
                {
                    _pooledObjects.Enqueue(CreateNewObject());
                }
            }
        }

        private ChunkView CreateNewObject()
        {
            ChunkView obj = GameObject.Instantiate(_chunkPrefabs[Random.Range(0, _chunkPrefabs.Length)], _poolRoot);
            obj.gameObject.SetActive(false);
            return obj;
        }
        
        public ChunkView Get(Vector3 position)
        {
            ChunkView obj;
        
            if (_pooledObjects.Count > 0)
            {
                obj = _pooledObjects.Dequeue(); 
            }
            else
            {
                obj = CreateNewObject();
            }
            
            obj.transform.SetPositionAndRotation(position, Quaternion.identity);
            obj.transform.SetParent(_chunkParent);
            obj.gameObject.SetActive(true);
        
            return obj;
        }

        public void Return(ChunkView obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolRoot);
            _pooledObjects.Enqueue(obj);
        }
    }
}