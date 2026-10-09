using _Project.Scripts.Audio;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using UnityEngine;

namespace _Project.Scripts.StateMachine
{
    public class SecondMapSectionState : IGameState
    {
        private MovementStats _stats;
        private MovementSettings _settings;
        private MapContextView _mapContextView;
        private AudioSystem _audioSystem;
        public SecondMapSectionState(MovementStats stats,  MovementSettings settings,  MapContextView mapContextView, AudioSystem audioSystem)
        {
            _stats = stats;
            _settings = settings;
            _mapContextView = mapContextView;
            _audioSystem = audioSystem;
        }
        public void Enter()
        {
            _audioSystem.PlaySound(_mapContextView.SecondMapSectionEnterSFX);
            _stats.SetWind(_settings.InitialWindDrag);
            _mapContextView.FastWind.SetActive(true);
            _mapContextView.FastWind.transform.position = _mapContextView.FastWindStartPos.position;
        }

        public void Tick(float deltaTime)
        {
            _stats.FadeWindTo(_settings.SustainedWindDrag, _settings.WindFadeRate * deltaTime);
            MoveFastWind();
        }

        public void Exit()
        {
            _mapContextView.FastWind.SetActive(false);
            _stats.ClearWind();
        }

        private void MoveFastWind()
        {
            Debug.Log("Move Fast Wind");
            
            _mapContextView.FastWind.transform.position = 
                Vector3.Lerp(_mapContextView.FastWind.transform.position, 
                    _mapContextView.FastWindSustainedPos.position, 
                    _mapContextView.FastWindSmooth);
        }
    }
}