using UnityEngine;

namespace Game.Boss
{
    public interface IBossState
    {
        void Enter(BossController boss);
        void Update(BossController boss);
        void Exit(BossController boss);
    }
}