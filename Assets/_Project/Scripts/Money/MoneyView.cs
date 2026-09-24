using TMPro;
using UnityEngine;

namespace _Project.Scripts.Money
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyText;

        public void UpdateMoneyText(int money)
        {
            moneyText.SetText($"Money: {money}");
        }
    }
}