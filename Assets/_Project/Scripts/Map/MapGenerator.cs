using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Map
{
    public class MapGenerator : IGameTick, IGameStart, IDisposable
    {
        private MovementView _movementView;
        private Transform spawnPosition;
        private ObjectPool _objectPool;
        private MapGeneratorView _mapGeneratorView;
        
        private readonly List<ChunkView> _activeChunks = new List<ChunkView>();
        
        public IReadOnlyList<ChunkView> ActiveChunks => _activeChunks;
        

        public MapGenerator(MovementView movementView, MapContextView mapContextView, ObjectPool objectPool, MapGeneratorView generatorView)
        {
            _movementView = movementView;
            spawnPosition = mapContextView.SpawnPosition;
            _objectPool = objectPool;
            _mapGeneratorView = generatorView;
        }

        public void Tick(float deltaTime)
        {

        }

        public void Start()
        {
            _movementView.DestroyTriggered += OnDestroyTriggered;
            _movementView.SpawnTriggered += OnSpawnTriggered;
            
            SpawnChunk();
        }

        public void Dispose()
        {
            _movementView.DestroyTriggered -= OnDestroyTriggered;
            _movementView.SpawnTriggered -= OnSpawnTriggered;
        }

        private void OnDestroyTriggered()
        {
            var chunk = _activeChunks[0];
            _activeChunks.RemoveAt(0);
            
            _objectPool.Return(chunk);
        }

        private void OnSpawnTriggered()
        {
            SpawnChunk();
        }

        private void SpawnChunk()
        {
            var obj = _objectPool.Get(GetSpawnPosition());
            _activeChunks.Add(obj);
            
        }

        private Vector3 GetSpawnPosition()
        {
            if (_activeChunks.Count == 0)
            {
                return spawnPosition.position;
            }
            
            var lastChunk = _activeChunks[^1];
            var newChunkPos = lastChunk.transform.position;
            newChunkPos.x += Random.Range(_mapGeneratorView.MinSpace, _mapGeneratorView.MaxSpace) + lastChunk.Length;
            newChunkPos.y = Random.Range(_mapGeneratorView.MinHeight, _mapGeneratorView.MaxHeight);
            
            //pawnPlatform();
            
            return newChunkPos;
        }

        private void SpawnPlatform(ChunkView obj)
        {
            Vector3 platformPosition = obj.transform.position;
            
            platformPosition.x += Random.Range(0, obj.Length);
        }
    }
}