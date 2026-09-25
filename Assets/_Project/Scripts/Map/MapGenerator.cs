using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using _Project.Scripts.Core;
using _Project.Scripts.Movement;
using _Project.Scripts.ObjectPools;
using _Project.Scripts.Player;
using Unity.AppUI.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Map
{
    public class MapGenerator : IGameTick, IGameStart, IDisposable
    {
        private MovementView _movementView;
        private Transform spawnPosition;
        private ChunkPool _chunkPool;
        private PlatformPool _platformPool;
        private MapGeneratorView _mapGeneratorView;
        
        private readonly List<ChunkView> _activeChunks = new List<ChunkView>();
        
        public IReadOnlyList<ChunkView> ActiveChunks => _activeChunks;
        

        public MapGenerator(MovementView movementView, MapContextView mapContextView, ChunkPool chunkPool, MapGeneratorView generatorView, PlatformPool platformPool)
        {
            _movementView = movementView;
            spawnPosition = mapContextView.SpawnPosition;
            _chunkPool = chunkPool;
            _platformPool = platformPool;
            _mapGeneratorView = generatorView;
        }

        public void Tick(float deltaTime)
        {

        }

        public void Start()
        {
            _movementView.DestroyTriggered += OnDestroyTriggered;
            _movementView.SpawnTriggered += OnSpawnTriggered;
            
            ResetMap();
        }

        public void ResetMap()
        {
            while (_activeChunks.Count > 0)
            {
                ReturnChunk(_activeChunks[0]);
            }
            
            SpawnChunk();
            SpawnChunk();
            SpawnChunk();
        }

        public void Dispose()
        {
            _movementView.DestroyTriggered -= OnDestroyTriggered;
            _movementView.SpawnTriggered -= OnSpawnTriggered;
        }

        private void OnDestroyTriggered()
        {
            ReturnChunk(_activeChunks[0]);
        }

        private void OnSpawnTriggered()
        {
            SpawnChunk();
        }

        private void SpawnChunk()
        {
            var obj = _chunkPool.Get(GetSpawnPosition(), _mapGeneratorView.ChunkParent);
            
            int topPlatforms = Random.Range(_mapGeneratorView.MinPlatformsTop, _mapGeneratorView.MaxPlatformsTop);
            int bottomPlatform = Random.Range(_mapGeneratorView.MinPlatformsBottom, _mapGeneratorView.MaxPlatformsBottom);

            for (int i = 0; i < topPlatforms; i++)
            {
                SpawnPlatform(obj, MapLayer.ROOFS);
            }

            for (int i = 0; i < bottomPlatform; i++)
            {
                SpawnPlatform(obj, MapLayer.BUILDING);
            }

            _activeChunks.Add(obj);
        }

        private void ReturnChunk(ChunkView chunk)
        {
            for (int i = chunk.Platforms.childCount - 1; i >= 0; i--)
            {
                if (chunk.Platforms.GetChild(i).TryGetComponent<PlatformView>(out var platform))
                    _platformPool.Return(platform);
            }
            
            _activeChunks.Remove(chunk);
            _chunkPool.Return(chunk);
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
            
            return newChunkPos;
        }

        private void SpawnPlatform(ChunkView obj, MapLayer layer)
        {
            Vector3 platformPosition = obj.transform.position;
            
            platformPosition.x += Random.Range(0, obj.Length);
            
            var platform = _platformPool.Get(layer, platformPosition, obj.Platforms);
            
            float height = 0;

            switch (layer)
            {
                case MapLayer.BUILDING:
                    height = obj.BuildingFloorYHeight;
                    break;
                case MapLayer.ROOFS:
                    height = obj.RoofsFloorYHeight;
                    break;
            }
            
            platform.transform.localPosition += Vector3.up*height;
        }
    }
}