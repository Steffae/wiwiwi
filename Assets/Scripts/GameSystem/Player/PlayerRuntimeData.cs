using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "PlayerRuntimeData", menuName = "Game/Player/Runtime Data")]
    public class PlayerRuntimeData : ScriptableObject
    {
        [Header("Health")]
        public float currentHealth = 300f;
        public float maxHealth = 300f;

        [Header("Position")]
        public Vector3 lastPosition;
        public string lastSceneName;

        [Header("Stats")]
        public float physicalDamage = 15f;
        public float magicDamage = 20f;

        [Header("Settings")]
        [SerializeField] private bool resetOnStart = true;

        public bool isInitialized = false;

        // Полный сброс данных (для новой игры)
        public void FullReset()
        {
            currentHealth = maxHealth;
            lastPosition = Vector3.zero;
            lastSceneName = "";
            physicalDamage = 15f;
            magicDamage = 20f;
            isInitialized = true;

            Debug.Log($"PlayerRuntimeData RESET: HP = {currentHealth}/{maxHealth}");
        }

        // Сброс при смерти (только HP и позиция)
        public void DeathReset()
        {
            currentHealth = maxHealth;
            lastPosition = Vector3.zero;

            Debug.Log($"PlayerRuntimeData DEATH RESET: HP = {currentHealth}/{maxHealth}");
        }

        public void ResetToDefault()
        {
            FullReset();
        }

        public float GetHealthPercent()
        {
            return currentHealth / maxHealth;
        }

        public void SavePosition(Vector3 position, string sceneName)
        {
            lastPosition = position;
            lastSceneName = sceneName;
        }
    }
}