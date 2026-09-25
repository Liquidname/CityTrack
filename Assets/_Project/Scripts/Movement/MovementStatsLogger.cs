using System.Collections.Generic;
using System.Text;
using _Project.Scripts.Core;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using _Project.Scripts.PTS;
using UnityEngine;

#if UNITY_EDITOR
namespace _Project.Scripts.Movement
{
    public class MovementStatsLogger : IGameTick, IGameStart, System.IDisposable
    {
        private readonly MovementSystem _movementSystem;
        private readonly PlayerView _playerView;
        private readonly ScoreSystem _scoreSystem;
        private readonly PlayerSystem _playerSystem;

        private float _timeSinceLastSample;
        private readonly float _sampleRate = 0.5f;

        // Velocity extremes
        private float _maxVelocityX;
        private float _maxVelocityY;
        private float _minVelocityY;

        // Height extremes
        private float _maxHeight;
        private float _minHeight;

        // Time per FlightState
        private float _timeDive;
        private float _timeGlide;

        // Score
        private float _maxScore;

        private int _platformBounces;
        private int _obstacleHits;

        private float _runStartTime;
        private bool _isRunning;

        private List<string> _eventsLog = new List<string>();
        private List<string> _telemetryLog = new List<string>();

        public MovementStatsLogger(MovementSystem movementSystem, PlayerView playerView,
            ScoreSystem scoreSystem, PlayerSystem playerSystem)
        {
            _movementSystem = movementSystem;
            _playerView = playerView;
            _scoreSystem = scoreSystem;
            _playerSystem = playerSystem;
        }

        public void Start()
        {
            _playerView.HitThePlatform += OnPlatformHit;
            _playerView.HitTheObstacle += OnObstacleHit;
            _movementSystem.Defeated += OnDefeated;
            Debug.Log("[MovementStatsLogger] Initialized. Waiting for movement...");
        }

        public void Tick(float deltaTime)
        {
            if (_movementSystem.VelocityX > 0 && !_isRunning)
            {
                _isRunning = true;
                _runStartTime = Time.time;
                float startHeight = _playerView.transform.position.y;
                _maxHeight = startHeight;
                _minHeight = startHeight;
                _eventsLog.Add($"[{GetRunTime():F2}s] RUN STARTED. VelX: {_movementSystem.VelocityX:F2}, Height: {startHeight:F2}");
            }

            if (!_isRunning) return;

            float velX = _movementSystem.VelocityX;
            float velY = _movementSystem.VelocityY;
            float height = _playerView.transform.position.y;
            FlightState state = _movementSystem.FlightState;
            MapLayer level = _playerSystem.FlightLevel;
            float score = _scoreSystem.Score;

            // Velocity extremes
            _maxVelocityX = Mathf.Max(_maxVelocityX, velX);
            _maxVelocityY = Mathf.Max(_maxVelocityY, velY);
            _minVelocityY = Mathf.Min(_minVelocityY, velY);

            // Height extremes
            _maxHeight = Mathf.Max(_maxHeight, height);
            _minHeight = Mathf.Min(_minHeight, height);

            // Score extreme
            _maxScore = Mathf.Max(_maxScore, score);

            // Time per state
            if (state == FlightState.DIVE)
                _timeDive += deltaTime;
            else
                _timeGlide += deltaTime;

            _timeSinceLastSample += deltaTime;
            if (_timeSinceLastSample >= _sampleRate)
            {
                _timeSinceLastSample = 0;
                _telemetryLog.Add(
                    $"t={GetRunTime():F1}s | {state,-5} | {level,-8} | VelX: {velX:F2} | VelY: {velY:F2} | Height: {height:F2} | Score: {score:F0}");
            }
        }

        private void OnPlatformHit(PlatformView platform)
        {
            if (!_isRunning) return;
            _platformBounces++;
            _eventsLog.Add(
                $"[{GetRunTime():F2}s] PLATFORM BOUNCE | {_movementSystem.FlightState,-5} | {_playerSystem.FlightLevel,-8} | " +
                $"VelX: {_movementSystem.VelocityX:F2}, VelY: {_movementSystem.VelocityY:F2}, " +
                $"Height: {_playerView.transform.position.y:F2}, Mult: {platform.PlatformBoostMultiplier:F2}, Score: {_scoreSystem.Score:F0}");
        }

        private void OnObstacleHit(Vector2 normal)
        {
            if (!_isRunning) return;
            _obstacleHits++;
            _eventsLog.Add(
                $"[{GetRunTime():F2}s] OBSTACLE HIT | {_movementSystem.FlightState,-5} | {_playerSystem.FlightLevel,-8} | " +
                $"Normal: {normal}, VelX: {_movementSystem.VelocityX:F2}, VelY: {_movementSystem.VelocityY:F2}, " +
                $"Height: {_playerView.transform.position.y:F2}, Score: {_scoreSystem.Score:F0}");
        }

        private void OnDefeated()
        {
            if (!_isRunning) return;
            _eventsLog.Add(
                $"[{GetRunTime():F2}s] DEFEAT TRIGGERED | {_movementSystem.FlightState,-5} | {_playerSystem.FlightLevel,-8} | " +
                $"VelX: {_movementSystem.VelocityX:F2}, VelY: {_movementSystem.VelocityY:F2}, " +
                $"Height: {_playerView.transform.position.y:F2}, Score: {_scoreSystem.Score:F0}");
            PrintLog();
            _isRunning = false;
        }

        public void Dispose()
        {
            _playerView.HitThePlatform -= OnPlatformHit;
            _playerView.HitTheObstacle -= OnObstacleHit;
            _movementSystem.Defeated -= OnDefeated;
        }

        private float GetRunTime() => Time.time - _runStartTime;

        public void PrintLog()
        {
            float totalTime = GetRunTime();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("======= RUN STATISTICS =======");
            sb.AppendLine($"Duration:         {totalTime:F2}s");
            sb.AppendLine($"Time in DIVE:     {_timeDive:F2}s ({(totalTime > 0 ? _timeDive / totalTime * 100 : 0):F1}%)");
            sb.AppendLine($"Time in GLIDE:    {_timeGlide:F2}s ({(totalTime > 0 ? _timeGlide / totalTime * 100 : 0):F1}%)");
            sb.AppendLine($"Max Vel X:        {_maxVelocityX:F2}");
            sb.AppendLine($"Max Vel Y (Up):   {_maxVelocityY:F2}");
            sb.AppendLine($"Min Vel Y (Down): {_minVelocityY:F2}");
            sb.AppendLine($"Max Height:       {_maxHeight:F2}");
            sb.AppendLine($"Min Height:       {_minHeight:F2}");
            sb.AppendLine($"Height Range:     {_maxHeight - _minHeight:F2}");
            sb.AppendLine($"Final Score:      {_scoreSystem.Score:F0}");
            sb.AppendLine($"Max Score:        {_maxScore:F0}");
            sb.AppendLine($"High Score:       {_scoreSystem.HighScore:F0}");
            sb.AppendLine($"Total Bounces:    {_platformBounces}");
            sb.AppendLine($"Obstacle Hits:    {_obstacleHits}");

            sb.AppendLine("\n--- EVENTS ---");
            foreach (var e in _eventsLog)
                sb.AppendLine(e);

            sb.AppendLine("\n--- TELEMETRY (0.5s interval) ---");
            foreach (var t in _telemetryLog)
                sb.AppendLine(t);

            sb.AppendLine("==============================");
            Debug.Log(sb.ToString());

            _eventsLog.Clear();
            _telemetryLog.Clear();
            _maxVelocityX = 0; _maxVelocityY = 0; _minVelocityY = 0;
            _maxHeight = 0; _minHeight = 0;
            _timeDive = 0; _timeGlide = 0;
            _maxScore = 0;
            _platformBounces = 0; _obstacleHits = 0;
        }
    }
}
#endif
