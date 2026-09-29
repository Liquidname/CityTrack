using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Upgrades
{
    [Serializable]
    public class UpgradeSlot
    {
        public UpgradeID id;
        public Button buyButton;
        public TMP_Text costText;
        public TMP_Text levelText;
    }
    
    public class UpgradeView : MonoBehaviour
    {
        [SerializeField] private GameObject _shopObject;
        [SerializeField] private UpgradeSlot[] upgradeSlots;
        private readonly Dictionary<UpgradeID, UpgradeSlot> _slotsById = new();
        public event Action<UpgradeID> buttonPressed;
        
        public GameObject ShopObject => _shopObject;
        private void Awake()
        {
            foreach (var slot in upgradeSlots)
            {
                _slotsById[slot.id] = slot;
                slot.buyButton.onClick.AddListener(() => buttonPressed?.Invoke(slot.id));
            }
        }
        public void UpdateSlotUI(UpgradeID id, int level, int cost, bool isMaxLevel)
        {
            if (!_slotsById.TryGetValue(id, out var slot))
            {
                Debug.LogError($"UpgradeSlot {id} not found");
                return;
            }
            slot.levelText.SetText($"{level}");
            slot.costText.SetText(isMaxLevel ? "MAX" : $"{cost}");
            slot.buyButton.interactable = !isMaxLevel;
        }
    }
}