namespace _Project.Scripts.Money
{
    public class MoneySystem
    {
        public int Money { get; private set; }

        public void AddMoney(int money)
        {
            Money += money;
        }

        public int getMoney(int summ)
        {
            if(Money < summ) return 0;
            
            Money -= summ;
            return summ;
        }
    }
}