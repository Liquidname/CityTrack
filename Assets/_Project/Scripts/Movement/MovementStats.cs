using System.Collections.Generic;
using _Project.Scripts.Map;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    /// <summary>
    /// Модификаторы движения от апгрейдов магазина.
    /// Нейтральные значения: бонусы = 0, множители = 1.
    /// Финальное значение вычисляется в MovementSystem: base (из MovementSettings) OP modifier.
    /// </summary>
    public class MovementStats
    {
        public float MaxSpeedBonus = 0f;
        public float ArmorProgress = 0f;
        public Dictionary<PlatformConfig, float> PlatformBounceBonus  = new();
        
        public float WindDrag { get; private set; }

        public void SetWind(float drag) => WindDrag = drag;
        public void ClearWind() => WindDrag = 0f;
        public void FadeWindTo(float target, float step)
        {
            WindDrag = Mathf.MoveTowards(WindDrag, target, step);
        }
        
        public float GetPlatformBounceBonus(PlatformConfig config)
        {
            if (config != null && PlatformBounceBonus.TryGetValue(config, out var bonus))
            {
                return bonus;
            }
            return 0f;
        }
    }
}