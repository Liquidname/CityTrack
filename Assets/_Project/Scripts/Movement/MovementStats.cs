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
    }
}