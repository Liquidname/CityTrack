using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.ObjectPools
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T[] _prefabs;
        private readonly Transform _poolRoot;
        private readonly Queue<T> _pooledObjects = new Queue<T>();
        
        private int initialCapacity = 5;

        public ObjectPool(T[] prefabs, Transform poolRoot)
        {
            _prefabs = prefabs;
            _poolRoot = poolRoot;
            
            for (int i = 0; i < _prefabs.Length; i++)
            {
                for (int j = 0; j < initialCapacity; j++)
                {
                    _pooledObjects.Enqueue(CreateNewObject());
                }
            }
        }

        private T CreateNewObject()
        {
            T obj = Object.Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], _poolRoot);
            obj.gameObject.SetActive(false);
            return obj;
        }
        
        public T Get(Vector3 position, Transform parent)
        {
            T obj;
        
            if (_pooledObjects.Count > 0)
            {
                obj = _pooledObjects.Dequeue(); 
            }
            else
            {
                obj = CreateNewObject();
            }
            
            obj.transform.SetPositionAndRotation(position, Quaternion.identity);
            
            obj.transform.SetParent(parent);
            
            obj.gameObject.SetActive(true);
        
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolRoot);
            _pooledObjects.Enqueue(obj);
        }
    }
}