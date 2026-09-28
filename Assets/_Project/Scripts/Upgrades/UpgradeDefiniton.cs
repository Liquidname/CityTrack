using System;
using UnityEngine;

namespace _Project.Scripts.Upgrades
{
    [Serializable]
    public struct UpgradeDefiniton
    {
        public UpgradeID id;
        public string DisplayName;
        public int MaxLevel;
        public float BaseCost;
        public float CostGrowthRate;
        public float ValuePerLevel;

        public int GetCost(int currentLevel) => Mathf.CeilToInt(BaseCost * Mathf.Pow(CostGrowthRate, currentLevel));

        public float CalculateValue(int level)
        {
            if (level <= 0) return 0f;

            if (id == UpgradeID.ARMOR)
            {
                if (MaxLevel <= 0) return 0f;
                float t = Mathf.Clamp01((float)level / MaxLevel);
                return 1f - (1f - t) * (1f - t);
            }

            return ValuePerLevel * level;
        }
    }
}