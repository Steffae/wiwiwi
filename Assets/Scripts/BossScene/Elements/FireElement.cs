using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class FireElement : BossElementBase
    {
        private GameObject fireEffectPrefab;
        private GameObject fireProjectilePrefab;
        private GameObject fireFlashPrefab;
        private float normalBurnDuration;
        private float heavyBurnDuration;
        private float flashNormalDuration;
        private float flashHeavyDuration;

        public override void Initialize(IAudioService audioService, BossController bossController, BossCombat bossCombat, BossStats bossStats)
        {
            base.Initialize(audioService, bossController, bossCombat, bossStats);

            // Берём настройки из Stats
            fireEffectPrefab = stats.fireEffectPrefab;
            fireProjectilePrefab = stats.fireProjectilePrefab;
            fireFlashPrefab = stats.fireFlashPrefab;
            normalBurnDuration = stats.fireNormalBurnDuration;
            heavyBurnDuration = stats.fireHeavyBurnDuration;
            flashNormalDuration = stats.fireFlashNormalDuration;
            flashHeavyDuration = stats.fireFlashHeavyDuration;
        }

        public override void ApplyMeleeEffect(GameObject target)
        {
            if (fireFlashPrefab != null && combat != null)
            {
                Transform meleePoint = combat.GetMeleeAttackPoint();
                if (meleePoint != null)
                {
                    GameObject flash = Object.Instantiate(fireFlashPrefab, meleePoint.position, Quaternion.identity);
                    boss.StartCoroutine(DestroyAfterTime(flash, flashNormalDuration));
                }
            }
        }

        public override void ApplyHeavyMeleeEffect(GameObject target)
        {
            if (fireFlashPrefab != null && combat != null)
            {
                Transform meleePoint = combat.GetMeleeAttackPoint();
                if (meleePoint != null)
                {
                    GameObject flash = Object.Instantiate(fireFlashPrefab, meleePoint.position, Quaternion.identity);
                    boss.StartCoroutine(DestroyAfterTime(flash, flashHeavyDuration));
                }
            }
        }

        public override void ApplyRangedEffect(GameObject target)
        {
            if (fireEffectPrefab != null && target != null)
            {
                GameObject fire = Object.Instantiate(fireEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
                boss.StartCoroutine(DestroyAfterTime(fire, normalBurnDuration));
            }
        }

        public override void ApplyHeavyRangedEffect(GameObject target)
        {
            if (fireEffectPrefab != null && target != null)
            {
                GameObject fire = Object.Instantiate(fireEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
                boss.StartCoroutine(DestroyAfterTime(fire, heavyBurnDuration));
            }
        }

        private IEnumerator DestroyAfterTime(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null) Object.Destroy(obj);
        }

        public override GameObject GetProjectilePrefab() => fireProjectilePrefab;
        public override Color GetElementColor() => stats.fireColor;
    }
}