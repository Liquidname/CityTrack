using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Money
{
    public class MoneySystem
    {
        private MoneyView _moneyView;
        public int Money { get; private set; }

        public MoneySystem(MoneyView moneyView)
        {
            _moneyView = moneyView;
#if UNITY_EDITOR
            AddMoney(1000);
#endif
        }
        
        public void AddMoney(int money)
        {
            Debug.Log($"Adding money {money}");
            Money += money;
            _moneyView.UpdateMoneyText(Money);
        }

        public bool TrySpendMoney(int summ)
        {
            if(Money < summ) return false;
            
            Money -= summ;
            _moneyView.UpdateMoneyText(Money);
            return true;
        }
    }
}