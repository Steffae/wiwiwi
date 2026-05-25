using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;
    public string actionMapName = "DefaultMap";

    [Header("Target")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 2f, -4f);

    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("Distance")]
    public float cameraDistance = 4f;

    private InputAction lookAction;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector2 lookInput;

    void Awake()
    {
        var actionMap = inputActions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"Action Map '{actionMapName}' not found!");
            return;
        }

        lookAction = actionMap.FindAction("Look");
    }

    void OnEnable()
    {
        lookAction.performed += OnLook;
        lookAction.canceled += OnLookCanceled;
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLookCanceled;
        lookAction.Disable();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = player.position + offset;
    }

    void Update()
    {
        if (player == null) return;

        // Поворот камеры мышью
        yRotation += lookInput.x * mouseSensitivity * Time.deltaTime;
        xRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;

        // Ограничение угла по вертикали
        xRotation = Mathf.Clamp(xRotation, -20f, 100f);

        // Вычисление позиции
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        Vector3 desiredPosition = player.position + rotation * new Vector3(offset.x, offset.y, -cameraDistance);

        // Проверка препятствий
        if (Physics.Linecast(player.position + Vector3.up * 1f, desiredPosition, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = desiredPosition;
        }

        // Направление камеры на игрока
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}