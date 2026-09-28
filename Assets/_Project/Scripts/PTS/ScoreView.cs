using TMPro;
using UnityEngine;

namespace _Project.Scripts.PTS
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText, chainText, highScoreText, multiplierText;
        [Range(0, 1f)]
        [SerializeField] private float ptsByMetrMultiplier = 0.1f;
        
        [Range(0, 35f)]
        [SerializeField] private float minimalVelocityXToExtraPTS = 25f;
        
        [SerializeField] private float roofsMultiplier = 1f;
        [SerializeField] private float inBuildingMultiplier = 1.4f;
        
        [SerializeField] private int chainReduceBySlowPlatformHit;
        
        [Header("Instant PTS")]
        [SerializeField] private float breakWindowInstantPTS;
        [SerializeField] private float platformBounceInstantPTS;
        [SerializeField] private float backToRoofsInstantPTS;
        
        public float PtsByMetrMultiplier => ptsByMetrMultiplier;
        public float MinimalVelocityXToExtraPTS => minimalVelocityXToExtraPTS;
        public int ChainReduceBySlowPlatformHit => chainReduceBySlowPlatformHit;
        public float RoofsMultiplier => roofsMultiplier;
        public float InBuildingMultiplier => inBuildingMultiplier;
        public float BreakWindowInstantPTS => breakWindowInstantPTS;
        public float PlatformBounceInstantPTS => platformBounceInstantPTS;

        public float BackToRoofsInstantPts => backToRoofsInstantPTS;

        public void UpdateScoreUI(int score)
        {
            scoreText.SetText($"PTS: {score}");
        }

        public void UpdateHighScoreUI(int highScore)
        {
            highScoreText.SetText($"High Score: {highScore}");
        }

        public void UpdateChainUI(int chain)
        {
            chainText.SetText($"Chain: {chain}");
        }

        public void UpdateMultiplierUI(float multiplier)
        {
            multiplierText.SetText($"Multiplier: {multiplier}");
        }
    }
}