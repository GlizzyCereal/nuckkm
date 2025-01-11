using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 1000f;

    private Camera mainCamera;
    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded = true;
    
    [Header("Sprint Settings")]
    public float maxSprintValue = 100f;
    public float currentSprintValue;
    public float sprintDepletionRate = 20f;
    public float sprintRegenerationRate = 10f;
    public Image sprintBarImage;
    [SerializeField] private float sprintTransitionSpeed = 5f;
    private bool isSprinting = false;
    private float targetSprintSpeed;

    [Header("Jump settings")]
    [SerializeField] private float jumpCooldown = 0.5f;
    public float jumpSprintCost = 10f;
    private float jumpTimer;

    [Header("Audio")]
    [SerializeField] private AudioClip sprintSound;
    private AudioSource audioSource;

    [Header("Health Settings")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private float healthBarTransitionSpeed = 5f;
    [SerializeField] private TextMeshProUGUI healthText;
    private Health playerHealth;
    private float targetHealthValue;
    private float displayedHealth;

    [Header("Hunger Settings")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float currentHunger;
    [SerializeField] private float hungerDepletionRate = 2f;
    [SerializeField] private float hungerDamageRate = 5f;
    [SerializeField] private Image hungerBarImage;
    [SerializeField] private float hungerBarTransitionSpeed = 5f;

    /*
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    private bool isInventoryOpen = false;
    */

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<Health>();

        Cursor.lockState = CursorLockMode.Locked;
        currentSprintValue = maxSprintValue;
        currentHunger = maxHunger;

        if (mainCamera == null)
            Debug.LogError("Main camera not found!");
    }

    private void Update()
    {
        HandleMouseLook();
        HandleJump();
        HandleSprint();
        UpdateUI();
        HandleHunger();
        //HandleInventory();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        float currentSpeed = isSprinting ? Mathf.Lerp(moveSpeed, targetSprintSpeed, Time.deltaTime * sprintTransitionSpeed) : moveSpeed;
        rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJump()
    {
        jumpTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded && jumpTimer <= 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpTimer = jumpCooldown;

            if (isSprinting)
            {
                currentSprintValue -= jumpSprintCost;
            }
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentSprintValue > 0)
        {
            isSprinting = true;
            targetSprintSpeed = sprintSpeed;
            if (audioSource && sprintSound)
                audioSource.PlayOneShot(sprintSound);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || currentSprintValue <= 0)
        {
            isSprinting = false;
            targetSprintSpeed = moveSpeed;
        }

        if (isSprinting && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            currentSprintValue -= sprintDepletionRate * Time.deltaTime;
            currentSprintValue = Mathf.Max(0, currentSprintValue);
        }
        else if (!isSprinting && currentSprintValue < maxSprintValue)
        {
            currentSprintValue += sprintRegenerationRate * Time.deltaTime;
            currentSprintValue = Mathf.Min(maxSprintValue, currentSprintValue);
        }
    }

    private void HandleHunger()
    {
        currentHunger -= hungerDepletionRate * Time.deltaTime;
        currentHunger = Mathf.Max(0, currentHunger);

        if (currentHunger <= 0)
        {
            playerHealth.TakeDamage(hungerDamageRate * Time.deltaTime);
        }
    }

    private void UpdateUI()
    {
        // Update sprint bar
        if (sprintBarImage != null)
        {
            sprintBarImage.fillAmount = currentSprintValue / maxSprintValue;
        }

        // Update hunger bar
        if (hungerBarImage != null)
        {
            hungerBarImage.fillAmount = Mathf.Lerp(hungerBarImage.fillAmount, currentHunger / maxHunger, Time.deltaTime * hungerBarTransitionSpeed);
        }

        // Update health bar and text
        if (playerHealth != null)
        {
            targetHealthValue = playerHealth.GetCurrentHealth();
            displayedHealth = Mathf.Lerp(displayedHealth, targetHealthValue, Time.deltaTime * healthBarTransitionSpeed);
        
            float currentMaxHealth = playerHealth.GetMaxHealth();
            float normalizedHealth = displayedHealth / currentMaxHealth;
        
            healthBarImage.fillAmount = normalizedHealth;
            healthText.text = $"{Mathf.Round(displayedHealth)}/{currentMaxHealth}";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    /*
    private void HandleInventory()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryUI.SetActive(isInventoryOpen);
            Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isInventoryOpen;
        }
    }
    */

    public void AddHunger(float amount)
    {
        currentHunger = Mathf.Min(currentHunger + amount, maxHunger);
    }
}