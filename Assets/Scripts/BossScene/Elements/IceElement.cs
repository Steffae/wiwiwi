using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class IceElement : BossElementBase
    {
        private GameObject iceProjectilePrefab;
        private GameObject iceEffectPrefab;
        private GameObject freezeEffectPrefab;
        private AudioClip iceSound;
        private float normalFreezeDuration;
        private float heavyFreezeDuration;
        private AudioSource audioSource;

        public override void Initialize(BossController bossController, BossCombat bossCombat, BossStats bossStats)
        {
            base.Initialize(bossController, bossCombat, bossStats);

            iceProjectilePrefab = stats.iceProjectilePrefab;
            iceEffectPrefab = stats.iceEffectPrefab;
            freezeEffectPrefab = stats.iceFreezeEffectPrefab;
            iceSound = stats.iceSound;
            normalFreezeDuration = stats.iceNormalFreezeDuration;
            heavyFreezeDuration = stats.iceHeavyFreezeDuration;

            audioSource = bossController.GetComponent<AudioSource>();
        }

        public override void ApplyMeleeEffect(GameObject target)
        {
            PlayIceSound();

            if (iceEffectPrefab != null && combat != null)
            {
                Transform meleePoint = combat.GetMeleeAttackPoint();
                if (meleePoint != null)
                {
                    GameObject effect = Object.Instantiate(iceEffectPrefab, meleePoint.position, Quaternion.identity);
                    Object.Destroy(effect, 2f);
                }
            }
        }

        public override void ApplyHeavyMeleeEffect(GameObject target)
        {
            ApplyMeleeEffect(target);
        }

        public override void ApplyRangedEffect(GameObject target)
        {
            boss.StartCoroutine(FreezePlayer(target, normalFreezeDuration));
        }

        public override void ApplyHeavyRangedEffect(GameObject target)
        {
            boss.StartCoroutine(FreezePlayer(target, heavyFreezeDuration));
        }

        private IEnumerator FreezePlayer(GameObject target, float duration)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player == null) yield break;

            // Замораживаем игрока
            player.enabled = false;

            // Создаём эффект заморозки на игроке
            GameObject freezeEffect = null;
            if (freezeEffectPrefab != null)
            {
                freezeEffect = Object.Instantiate(freezeEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
            }

            Debug.Log($"Player frozen for {duration} seconds!");

            yield return new WaitForSeconds(duration);

            // Размораживаем
            player.enabled = true;

            if (freezeEffect != null)
            {
                Object.Destroy(freezeEffect);
            }

            Debug.Log("Player unfrozen");
        }

        private void PlayIceSound()
        {
            if (audioSource != null && iceSound != null)
                audioSource.PlayOneShot(iceSound);
        }

        public override GameObject GetProjectilePrefab() => iceProjectilePrefab;
        public override Color GetElementColor() => stats.iceColor;
    }
}