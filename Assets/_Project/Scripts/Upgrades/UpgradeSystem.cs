using System.Collections.Generic;
using _Project.Scripts.Money;
using _Project.Scripts.Movement;
using UnityEngine;

namespace _Project.Scripts.Upgrades
{
    public class UpgradeSystem
    {
        private UpgradeConfig _config;
        private MoneySystem _moneySystem;
        private MovementStats _stats;
        private UpgradeView _upgradeView;
        
        private readonly Dictionary<UpgradeID, int> _levels = new();
        
        public int GetLevel(UpgradeID id) => _levels.TryGetValue(id, out var lv) ? lv : 0;

        public UpgradeSystem(UpgradeConfig config, MoneySystem moneySystem,  MovementStats stats,  UpgradeView upgradeView)
        {
            _config = config;
            _moneySystem = moneySystem;
            _stats = stats;
            _upgradeView = upgradeView;

            _upgradeView.buttonPressed += OnButtonPressed;
            RecalculateAllStats();
            UpdateAllViews();
        }

        private void OnButtonPressed(UpgradeID id)
        {
            Debug.Log("Buy Button Pressed");
            TryBuyUpgrade(id);
        } 
        
        private void UpdateAllViews()
        {
            foreach (var def in _config.definitons)
            {
                UpdateSlotView(def);
            }
        }
        
        private void UpdateSlotView(UpgradeDefiniton def)
        {
            int level = GetLevel(def.id);
            bool isMax = level >= def.MaxLevel;
            int cost = def.GetCost(level);
            _upgradeView.UpdateSlotUI(def.id, level, cost, isMax);
        }
        
        private bool TryBuyUpgrade(UpgradeID id)
        {
            var definiton = _config.Get(id);
            int currentLevel = GetLevel(id);
            
            if(currentLevel >= definiton.MaxLevel) return false;
            
            var cost = definiton.GetCost(currentLevel);
            if(_moneySystem.TrySpendMoney(cost) == false) return false;


            _levels[id] = currentLevel + 1;
            UpdateSlotView(definiton);
            ApplyStat(id, definiton.CalculateValue(GetLevel(id)));
            
            return true;
        }

        private void RecalculateAllStats()
        {
            Debug.Log($"Recalculating all stats");
            foreach (var settings in _config.definitons)
            {
                ApplyStat(settings.id, settings.CalculateValue(GetLevel(settings.id)));
            }
        }
        private void ApplyStat(UpgradeID id, float value)
        {
            Debug.Log($"Applying stat {id}");
            switch (id)
            {
                case UpgradeID.MAX_SPEED_BONUS:
                    _stats.MaxSpeedBonus = value;
                    break;
                case UpgradeID.ARMOR:
                    _stats.ArmorProgress = value;
                    break;
            }
        }
    }
}