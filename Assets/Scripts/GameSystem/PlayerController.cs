using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 8f;
    public float gravityMultiplier = 2f;
    public float groundRayLength = 1.2f; // длина луча

    [Header("Components")]
    private Rigidbody rb;
    private Animator animator;

    [Header("Combat")]
    public float physicalDamage = 15f;
    public float magicDamage = 20f;
    public float attackRange = 2f;
    public MeleeWeapon weapon;
    public GameObject fishPrefab;
    public float fishSpeed = 15f;
    public GameObject magicProjectilePrefab;

    // Input Action Asset
    public InputActionAsset inputActions;
    public string actionMapName = "PlayerControls";

    // Input Action references
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction magicAttackAction;

    // Input values
    private Vector2 moveInput;
    private bool runInput;
    private bool jumpPressed;
    private bool attackPressed;
    private bool magicPressed;

    // Movement variables
    private float currentSpeed;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isRunning;
    private bool isJumping;

    void Awake()
    {
        var actionMap = inputActions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"Action Map '{actionMapName}' not found!");
            return;
        }

        moveAction = actionMap.FindAction("Move");
        runAction = actionMap.FindAction("Run");
        jumpAction = actionMap.FindAction("Jump");
        attackAction = actionMap.FindAction("Attack");
        magicAttackAction = actionMap.FindAction("MagicAttack");
    }

    void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;
        runAction.performed += OnRun;
        runAction.canceled += OnRunCanceled;
        jumpAction.performed += OnJump;
        attackAction.performed += OnAttack;
        magicAttackAction.performed += OnMagicAttack;

        moveAction.Enable();
        runAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        magicAttackAction.Enable();
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;
        runAction.performed -= OnRun;
        runAction.canceled -= OnRunCanceled;
        jumpAction.performed -= OnJump;
        attackAction.performed -= OnAttack;
        magicAttackAction.performed -= OnMagicAttack;

        moveAction.Disable();
        runAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        magicAttackAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        runInput = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        runInput = false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            attackPressed = true;
    }

    private void OnMagicAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            magicPressed = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.mass = 1f;
        rb.linearDamping = 2f;
        rb.angularDamping = 5f;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleInput();
        HandleAttacks();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    void HandleGroundCheck()
    {
        // Круговая проверка земли вокруг персонажа
        Vector3 center = transform.position + Vector3.up * 0.5f;

        isGrounded = false;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * 0.5f, 0, Mathf.Sin(angle) * 0.5f);
            Vector3 rayStart = center + offset;

            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, groundRayLength))
            {
                Debug.DrawRay(rayStart, Vector3.down * groundRayLength, Color.green);
                isGrounded = true;
                break;
            }
            else
            {
                Debug.DrawRay(rayStart, Vector3.down * groundRayLength, Color.red);
            }
        }

        if (isGrounded && isJumping)
        {
            isJumping = false;
        }
    }

    void HandleInput()
    {
        isRunning = runInput && moveInput.y > 0;
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
    }

    void HandleMovement()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            Vector3 moveVelocity = moveDirection * currentSpeed;
            moveVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = moveVelocity;
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Vector3 slowdown = horizontalVelocity * (1 - Time.fixedDeltaTime * 5f);
                rb.linearVelocity = new Vector3(slowdown.x, rb.linearVelocity.y, slowdown.z);
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }

    void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            isJumping = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("JumpAir");
            jumpPressed = false;
            Debug.Log("Прыжок!");
        }
        else if (jumpPressed && !isGrounded)
        {
            Debug.Log("Не на земле, прыжок невозможен");
            jumpPressed = false;
        }
    }

    void HandleAttacks()
    {
        if (attackPressed)
        {
            animator.SetTrigger("Attack01");
            attackPressed = false;
            Debug.Log("Атака");
            PerformMeleeAttack();
        }

        if (magicPressed)
        {
            animator.SetTrigger("Attack02Start");
            magicPressed = false;
            Debug.Log("Магия");
            PerformMagicAttack();
        }
    }

    void PerformMeleeAttack()
    {
        if (weapon != null)
        {
            weapon.StartAttack();
            Debug.Log("Выполняется атака");
        }
    }

    void PerformMagicAttack()
    {
        if (fishPrefab != null)
        {
            // Точка появления снаряда
            Vector3 spawnPos = transform.position + transform.forward * 1.5f + Vector3.up * 1.2f;

            // Создание снаряда
            GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
            fish.tag = "PlayerProjectile";

            // Настройка физики
            Rigidbody rb = fish.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = fish.AddComponent<Rigidbody>();
            }

            rb.mass = 0.3f;
            rb.linearDamping = 0.2f;
            rb.angularDamping = 0.1f;
            rb.useGravity = true;

            // Настройка коллайдера
            if (fish.GetComponent<Collider>() == null)
            {
                SphereCollider col = fish.AddComponent<SphereCollider>();
                PhysicsMaterial mat = new PhysicsMaterial();
                mat.bounciness = 0.6f;
                col.material = mat;
            }

            // Настройка скрипта
            FishProjectile fishScript = fish.GetComponent<FishProjectile>();
            if (fishScript == null)
            {
                fishScript = fish.AddComponent<FishProjectile>();
            }
            fishScript.damage = magicDamage;
            fishScript.speed = fishSpeed;

            // Полёт с небольшой дугой
            rb.linearVelocity = transform.forward * fishSpeed + Vector3.up * 2f;

            Debug.Log("Рыба запущена!");
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void UpdateAnimations()
    {
        float horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        float normalizedSpeed = horizontalVelocity / runSpeed;

        animator.SetFloat("Speed", normalizedSpeed);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);
    }
}