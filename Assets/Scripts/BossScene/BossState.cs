namespace Game.Boss
{
    public enum BossState
    {
        Idle,           // Покой
        Chase,          // Преследование
        Attack,         // Обычная атака
        HeavyAttack,    // Сильная атака
        Stunned,        // Оглушение
        Flee,           // Бегство
        Enrage,         // Ярость
        Dead            // Смерть
    }
}