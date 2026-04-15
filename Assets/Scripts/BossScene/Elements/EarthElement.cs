using UnityEngine;
using System.Collections;

namespace Game.Boss
{
    public class EarthElement : BossElementBase
    {
        private GameObject earthProjectilePrefab;
        private GameObject mudEffectPrefab;
        private AudioClip earthSound;
        private float normalSlowDuration;
        private float heavySlowDuration;
        private float slowAmount;
        private float knockupForce;
        private IAudioService audioService;

        public override void Initialize(IAudioService audioService, BossController bossController, BossCombat bossCombat, BossStats bossStats)
        {
            base.Initialize(audioService, bossController, bossCombat, bossStats);

            earthProjectilePrefab = stats.earthProjectilePrefab;
            mudEffectPrefab = stats.earthMudEffectPrefab;
            earthSound = stats.earthSound;
            normalSlowDuration = stats.earthNormalSlowDuration;
            heavySlowDuration = stats.earthHeavySlowDuration;
            slowAmount = stats.earthSlowAmount;
            knockupForce = stats.earthKnockupForce;

            this.audioService = audioService;
        }

        public override void ApplyMeleeEffect(GameObject target)
        {
            PlayEarthSound();

            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(Vector3.up * knockupForce, ForceMode.Impulse);
        }

        public override void ApplyHeavyMeleeEffect(GameObject target)
        {
            PlayEarthSound();

            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(Vector3.up * knockupForce * 1.5f, ForceMode.Impulse);
        }

        public override void ApplyRangedEffect(GameObject target)
        {
            if (mudEffectPrefab != null && target != null)
            {
                GameObject mud = Object.Instantiate(mudEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
                boss.StartCoroutine(DestroyAfterTime(mud, normalSlowDuration));
            }
            boss.StartCoroutine(SlowPlayer(target, normalSlowDuration));
        }

        public override void ApplyHeavyRangedEffect(GameObject target)
        {
            if (mudEffectPrefab != null && target != null)
            {
                GameObject mud = Object.Instantiate(mudEffectPrefab, target.transform.position, Quaternion.identity, target.transform);
                boss.StartCoroutine(DestroyAfterTime(mud, heavySlowDuration));
            }
            boss.StartCoroutine(SlowPlayer(target, heavySlowDuration));
        }

        private IEnumerator SlowPlayer(GameObject target, float duration)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player == null) yield break;

            float origWalk = player.walkSpeed;
            float origRun = player.runSpeed;

            player.walkSpeed *= (1 - slowAmount);
            player.runSpeed *= (1 - slowAmount);

            yield return new WaitForSeconds(duration);

            player.walkSpeed = origWalk;
            player.runSpeed = origRun;
        }

        private IEnumerator DestroyAfterTime(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null) Object.Destroy(obj);
        }

        private void PlayEarthSound()
        {
            audioService.PlaySoundEffect(earthSound);
        }

        public override GameObject GetProjectilePrefab() => earthProjectilePrefab;
        public override Color GetElementColor() => stats.earthColor;
    }
}