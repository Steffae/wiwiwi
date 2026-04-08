using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);
    public GameObject AttackPanel;

    private HealthSystem healthSystem;
    private Slider healthSlider;
    private GameObject healthBarInstance;
    private RectTransform healthBarRect;
    private Camera mainCamera;

    public HealthSystem HealthSystem => healthSystem;

    void Start()
    {
        healthSystem = new HealthSystem(maxHealth);
        mainCamera = Camera.main;

        // Подписываемся на события
        healthSystem.OnHealthChanged += UpdateHealthUI;
        healthSystem.OnDeath += Die;

        // Создаём полоску здоровья
        if (healthBarPrefab != null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
            healthSlider = healthBarInstance.GetComponent<Slider>();
            healthBarRect = healthBarInstance.GetComponent<RectTransform>();

            if (healthSlider != null)
            {
                healthSlider.minValue = 0;
                healthSlider.maxValue = maxHealth;
                healthSlider.value = healthSystem.CurrentHealth;
            }
        }
    }

    void LateUpdate()
    {
        if (healthBarRect != null && mainCamera != null && healthSystem.CurrentHealth > 0)
        {
            Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
            bool isVisible = viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0;

            if (isVisible)
            {
                RaycastHit hit;
                Vector3 direction = transform.position - mainCamera.transform.position;
                if (Physics.Raycast(mainCamera.transform.position, direction, out hit, direction.magnitude))
                {
                    if (hit.collider.gameObject != gameObject)
                        isVisible = false;
                }
            }

            if (isVisible)
            {
                Vector3 worldPos = transform.position + healthBarOffset;
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                healthBarRect.position = screenPos;
                healthBarInstance.SetActive(true);
            }
            else
            {
                healthBarInstance.SetActive(false);
            }
        }
    }

    void UpdateHealthUI(float currentHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        healthSystem.TakeDamage(damage);
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось HP: {healthSystem.CurrentHealth}");

        // Анимация получения урона
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            AttackPanel.SetActive(true);
            StartCoroutine(HideAfterTime());
            anim.SetTrigger("GetHit");
        }
    }

    protected virtual IEnumerator HideAfterTime()
    {
        yield return new WaitForSeconds(0.5f);
        AttackPanel.SetActive(false);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} умер");

        Animator anim = GetComponent<Animator>();
        PlayerController controller = GetComponent<PlayerController>();

        if (anim != null && controller != null)
        {
            anim.SetTrigger("Die");
            DisableCameraControls();
            controller.enabled = false;
            SceneManager.LoadScene("End");
        }

        // Убираем полоску
        if (healthBarInstance != null)
            Destroy(healthBarInstance, 2f);

        Destroy(gameObject, 2f);
    }

    void DisableCameraControls()
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            ThirdPersonCamera camCtrl = cam.GetComponent<ThirdPersonCamera>();
            if (camCtrl != null) camCtrl.enabled = false;

            PlayerInput playerInput = cam.GetComponent<PlayerInput>();
            if (playerInput != null) playerInput.enabled = false;
        }
    }
}