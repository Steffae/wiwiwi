using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    public class PeacefulModeScrollbar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Text normalText;
        [SerializeField] private Text peaceText;

        [Header("Text Settings")]
        [SerializeField] private string peacefulText = "Мирный";
        [SerializeField] private string combatText = "Обычный";

        [Header("Colors")]
        [SerializeField] private Color peacefulColor = new Color(0.3f, 0.8f, 0.3f, 1f);
        [SerializeField] private Color combatColor = new Color(0.8f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color whiteColor = new Color(1f, 1f, 1f, 0.5f);

        [SerializeField] private Image handleImage;

        private void Awake()
        {
            if (scrollbar == null)
            {
                scrollbar = GetComponent<Scrollbar>();
            }

            // Настраиваем Scrollbar на два положения
            scrollbar.numberOfSteps = 2;
            scrollbar.value = 0f;
        }

        private void Start()
        {
            // Устанавливаем начальное значение из GameManager
            if (GameManager.Instance != null)
            {
                scrollbar.value = GameManager.Instance.IsPeacefulMode ? 1f : 0f;
                UpdateVisuals();
            }

            // Подписываемся на изменения Scrollbar
            scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        }

        private void OnScrollbarChanged(float value)
        {
            // Принудительно округляем до 0 или 1
            float snappedValue = Mathf.Round(value);

            // Если значение изменилось из-за округления - обновляем Scrollbar
            if (Mathf.Abs(scrollbar.value - snappedValue) > 0.01f)
            {
                scrollbar.value = snappedValue;
            }

            // Применяем режим
            bool isPeaceful = snappedValue >= 0.5f;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPeacefulMode(isPeaceful);
            }

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            bool isPeaceful = scrollbar.value >= 0.5f;

            if (isPeaceful)
            {
                peaceText.text = peacefulText;
                peaceText.color = peacefulColor;
                normalText.color = whiteColor;
            }

            if (!isPeaceful)
            {
                normalText.text = combatText;
                normalText.color = combatColor;
                peaceText.color = whiteColor;
            }

            if (handleImage != null)
            {
                handleImage.color = isPeaceful ? peacefulColor : combatColor;
            }
        }

        private void OnDestroy()
        {
            scrollbar.onValueChanged.RemoveListener(OnScrollbarChanged);
        }

        public bool IsPeacefulMode()
        {
            return scrollbar.value >= 0.5f;
        }
    }
}