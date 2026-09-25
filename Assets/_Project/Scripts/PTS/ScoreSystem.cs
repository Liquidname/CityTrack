using System;
using UnityEngine;
using _Project.Scripts.Core;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using Unity.VisualScripting;

namespace _Project.Scripts.PTS
{
    public class ScoreSystem : IGameTick, IGameStart, IDisposable
    {
        public float Score { get; private set; }
        public float HighScore { get; private set; }
        
        public event Action<int> OnScoreChanged;
        
        private MovementSystem _movementSystem;
        private ScoreView _scoreView;
        private PlayerView _playerView;
        private PlayerSystem _playerSystem;

        private int chain = 0;
        private float multiplier;
        
        public ScoreSystem(MovementSystem movementSystem,  ScoreView scoreView,  PlayerView playerView,  PlayerSystem playerSystem)
        {
            _movementSystem = movementSystem;
            _scoreView = scoreView;
            _playerView = playerView;
            _playerSystem = playerSystem;
        }


        public void Start()
        {
            _playerView.HitThePlatform += OnPlayerHitPlatform;
            _playerView.HitTheObstacle += OnPlayerHitObstacle;
            _playerView.BreakTheWindow += OnPlayerBreakWindow;
            
            ResetScore();
        }
        
        public void Dispose()
        {
            _playerView.HitThePlatform -= OnPlayerHitPlatform;
            _playerView.HitTheObstacle -= OnPlayerHitObstacle;
        }
        
        public void Tick(float deltaTime)
        {
            if (_movementSystem.VelocityX > 0)
            {
                AddScore((_movementSystem.VelocityX * deltaTime) * _scoreView.PtsByMetrMultiplier);
            }
        }

        public void ResetScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                _scoreView.UpdateHighScoreUI((int)HighScore);
            }
            
            Score = 0;
            multiplier = _scoreView.RoofsMultiplier;
            _scoreView.UpdateMultiplierUI(multiplier);
            _scoreView.UpdateScoreUI((int)Score);
        }

        private void OnPlayerHitPlatform(PlatformView platform)
        {
            if (_movementSystem.VelocityX > _scoreView.MinimalVelocityXToExtraPTS)
            { 
                PlayerFastHitPlatform();     
            }
            else if(_movementSystem.VelocityX <= _scoreView.MinimalVelocityXToExtraPTS)
            { 
                PlayerSlowHitPlatform();
            }

            if (platform.PlatformType == PlatformType.BUILDING_SAVER)
            {
                OnPlayerSaveFromLowLayer();
            }
        }

        private void OnPlayerHitObstacle(Vector2 obstacle)
        {
            chain=0;
            _scoreView.UpdateChainUI(chain);
        }
            
        private void AddScore(float value)
        {
            float chainMultiplier = 1 + 0.25f * chain;
            Score += value * chainMultiplier;
            OnScoreChanged?.Invoke((int)Score);
            _scoreView.UpdateScoreUI((int)Score);
        }

        private void PlayerFastHitPlatform()
        {
            AddScore(_scoreView.PlatformBounceInstantPTS);
            chain++;
            _scoreView.UpdateChainUI(chain);
        }

        private void PlayerSlowHitPlatform()
        {
            chain = Mathf.Max(0, chain-_scoreView.ChainReduceBySlowPlatformHit);
            _scoreView.UpdateChainUI(chain);
        }

        private void OnPlayerBreakWindow()
        {
            AddScore(_scoreView.BreakWindowInstantPTS);
            
            if (_playerSystem.FlightLevel == MapLayer.BUILDING)
            {
                multiplier = _scoreView.InBuildingMultiplier;
            } else if (_playerSystem.FlightLevel == MapLayer.ROOFS)
            {
                multiplier = _scoreView.RoofsMultiplier;
            }
            
            _scoreView.UpdateMultiplierUI(multiplier);
        }

        private void OnPlayerSaveFromLowLayer()
        {
            AddScore(_scoreView.BackToRoofsInstantPts);
        }
    }
}