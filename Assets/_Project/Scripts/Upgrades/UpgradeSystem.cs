using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Money;
using _Project.Scripts.Movement;
using UnityEngine;

namespace _Project.Scripts.Upgrades
{
    public class UpgradeSystem : IGameStart
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

            
        }
        
        public void Start()
        {
            _upgradeView.buttonPressed += OnButtonPressed;
            RecalculateAllStats();
            UpdateAllViews();
        }

        public void EnableShop(bool enable)
        {
            _upgradeView.ShopObject.SetActive(enable);
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
            ApplyStat(definiton, definiton.CalculateValue(GetLevel(id)));
            
            return true;
        }

        private void RecalculateAllStats()
        {
            Debug.Log($"Recalculating all stats");
            foreach (var definiton in _config.definitons)
            {
                ApplyStat(definiton, definiton.CalculateValue(GetLevel(definiton.id)));
            }
        }
        private void ApplyStat(UpgradeDefiniton definiton, float value)
        {
            var id = definiton.id;
            Debug.Log($"Applying stat {id}");

            if (definiton.platformConfig != null)
            {
                ApplyPlatformStat(definiton, value);
                return;
            }
            
            switch (id)
            {
                case UpgradeID.MaxSpeedBonus:
                    _stats.MaxSpeedBonus = value;
                    break;
                case UpgradeID.Armor:
                    _stats.ArmorProgress = value;
                    break;
                default:
                    Debug.LogError($"Unknown stat {id}");
                    break;
            }
        }

        private void ApplyPlatformStat(UpgradeDefiniton definition, float value)
        {
            //_stats.PlatformBounceBonus[definition.platformConfig] = value;
        }

        
    }
}