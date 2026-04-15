using UnityEngine;

namespace Game.Boss
{
    public abstract class BossElementBase : MonoBehaviour
    {
        protected IAudioService audioS;
        protected BossController boss;
        protected BossCombat combat;
        protected BossStats stats;

        public virtual void Initialize(IAudioService audioService, BossController bossController, BossCombat bossCombat, BossStats bossStats)
        {
            audioS = audioService;
            boss = bossController;
            combat = bossCombat;
            stats = bossStats;
        }

        // Ближняя атака (обычная)
        public abstract void ApplyMeleeEffect(GameObject target);

        // Ближняя атака (тяжёлая)
        public abstract void ApplyHeavyMeleeEffect(GameObject target);

        // Дальняя атака (обычная) - применяется к снаряду или при попадании
        public abstract void ApplyRangedEffect(GameObject target);

        // Дальняя атака (тяжёлая)
        public abstract void ApplyHeavyRangedEffect(GameObject target);

        // Получить префаб снаряда
        public abstract GameObject GetProjectilePrefab();

        // Получить цвет стихии
        public abstract Color GetElementColor();
    }
}