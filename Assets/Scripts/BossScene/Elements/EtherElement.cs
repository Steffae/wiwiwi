using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

namespace Game.Boss
{
    public class EtherElement : BossElementBase
    {
        private GameObject etherProjectilePrefab;
        private GameObject etherEffectPrefab;

        private AudioClip etherSound;

        private float normalInvisDuration;
        private float heavyInvisDuration;
        private Renderer[] bossRenderers;
        [SerializeField] private GameObject bossHealthBar;

        private IAudioService audioService;

        public override void Initialize(IAudioService audioService, BossController bossController, BossCombat bossCombat, BossStats bossStats)
        {
            base.Initialize(audioService, bossController, bossCombat, bossStats);

            etherProjectilePrefab = stats.etherProjectilePrefab;
            etherEffectPrefab = stats.etherEffectPrefab;
            etherSound = stats.etherSound;
            normalInvisDuration = stats.etherNormalInvisDuration;
            heavyInvisDuration = stats.etherHeavyInvisDuration;

            this.audioService = audioService;
            bossRenderers = bossController.GetComponentsInChildren<Renderer>();
        }

        public override void ApplyMeleeEffect(GameObject target)
        {
            PlayEtherSound();

            if (etherEffectPrefab != null && combat != null)
            {
                Transform meleePoint = combat.GetMeleeAttackPoint();
                if (meleePoint != null)
                    Object.Instantiate(etherEffectPrefab, meleePoint.position, Quaternion.identity);
            }
        }

        public override void ApplyHeavyMeleeEffect(GameObject target)
        {
            ApplyMeleeEffect(target);
        }

        public override void ApplyRangedEffect(GameObject target)
        {
            boss.StartCoroutine(MakeBossInvisible(normalInvisDuration));

            if (etherEffectPrefab != null && combat != null)
            {
                Transform rangedPoint = combat.GetRangedAttackPoint();
                if (rangedPoint != null)
                    Object.Instantiate(etherEffectPrefab, rangedPoint.position, Quaternion.identity);
            }
        }

        public override void ApplyHeavyRangedEffect(GameObject target)
        {
            boss.StartCoroutine(MakeBossInvisible(heavyInvisDuration));

            if (etherEffectPrefab != null && combat != null)
            {
                Transform rangedPoint = combat.GetRangedAttackPoint();
                if (rangedPoint != null)
                    Object.Instantiate(etherEffectPrefab, rangedPoint.position, Quaternion.identity);
            }
        }

        private IEnumerator MakeBossInvisible(float duration)
        {
            SetBossVisibility(false);
            bossHealthBar.SetActive(false);
            yield return new WaitForSeconds(duration);
            SetBossVisibility(true);
            bossHealthBar.SetActive(true);
        }

        private void SetBossVisibility(bool visible)
        {
            foreach (Renderer r in bossRenderers)
                if (r != null) r.enabled = visible;
        }

        private void PlayEtherSound()
        {
            if (audioService != null && etherSound != null)
            {
                audioService.PlaySoundEffect(etherSound);
            }
        }

        public override GameObject GetProjectilePrefab() => etherProjectilePrefab;
        public override Color GetElementColor() => stats.etherColor;
    }
}