using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "PlayerRuntimeData", menuName = "Game/Player/Runtime Data")]
    public class PlayerRuntimeData : ScriptableObject
    {
        [Header("Health")]
        public float currentHealth = 100f;
        public float maxHealth = 100f;

        [Header("Position")]
        public Vector3 lastPosition;
        public string lastSceneName;

        [Header("Stats")]
        public float physicalDamage = 15f;
        public float magicDamage = 20f;

        // Флаг для отслеживания, был ли игрок инициализирован
        public bool isInitialized = false;

        public void ResetToDefault()
        {
            currentHealth = maxHealth;
            lastPosition = Vector3.zero;
            lastSceneName = "";
            physicalDamage = 15f;
            magicDamage = 20f;
            isInitialized = true;
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