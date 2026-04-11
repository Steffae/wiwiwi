using UnityEngine;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Peaceful Mode")]
        [SerializeField] private bool isPeacefulMode = false;

        // Событие, на которое подписываются босс и мобы
        public System.Action<bool> OnPeacefulModeChanged;

        // Свойство для доступа к текущему режиму
        public bool IsPeacefulMode
        {
            get => isPeacefulMode;
            private set
            {
                if (isPeacefulMode != value)
                {
                    isPeacefulMode = value;
                    OnPeacefulModeChanged?.Invoke(isPeacefulMode);
                    Debug.Log($"Peaceful Mode changed to: {isPeacefulMode}");
                }
            }
        }

        private void Awake()
        {
            // Синглтон
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // При старте оповещаем всех о текущем режиме
            OnPeacefulModeChanged?.Invoke(isPeacefulMode);
        }

        // Установка мирного режима (0 = боевой, 1 = мирный)
        public void SetPeacefulMode(float value)
        {
            IsPeacefulMode = value >= 0.5f;
        }

        // Установка мирного режима (bool)
        public void SetPeacefulMode(bool value)
        {
            IsPeacefulMode = value;
        }

        // Переключение мирного режима
        public void TogglePeacefulMode()
        {
            IsPeacefulMode = !IsPeacefulMode;
        }

        // Подписка для врагов (босс, мобы)
        public void SubscribeToPeacefulMode(System.Action<bool> callback)
        {
            OnPeacefulModeChanged += callback;
            callback?.Invoke(isPeacefulMode);
        }
 
        // Отписка
        public void UnsubscribeFromPeacefulMode(System.Action<bool> callback)
        {
            OnPeacefulModeChanged -= callback;
        }
    }
}