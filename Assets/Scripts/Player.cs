using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 1000f;

    
    [Header("Sprint Settings")]
    public float maxSprintValue = 100f;
    public float currentSprintValue;
    public float sprintDepletionRate = 20f;
    public float jumpSprintCost = 10f;
    public float sprintRegenerationRate = 10f;
    public Image sprintBarImage;
    [SerializeField] private float sprintTransitionSpeed = 5f;


    [Header("Jump settings")]
    [SerializeField] private float jumpCooldown = 0.5f;


    [Header("Audio")]
    [SerializeField] private AudioClip sprintSound;
    private AudioSource audioSource;


    [Header("Health Settings")]
    [SerializeField] private Image healthBarImage;   // Reference to UI Image for health bar
    [SerializeField] private float healthBarTransitionSpeed = 5f;
    [SerializeField] private TMPro.TextMeshProUGUI healthText; // Reference to UI Text for health value
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


    private float jumpTimer;
    private float targetSprintSpeed;
    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded = true;
    private bool isSprinting = false;

    void Start()
    {
        currentSprintValue = maxSprintValue;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = GetComponent<AudioSource>();
        
        playerHealth = GetComponent<Health>();
        displayedHealth = playerHealth.GetCurrentHealth();
        targetHealthValue = displayedHealth;

        currentHunger = maxHunger;
        
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from Player!");
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleSprint();
        UpdateSprintBar();
        UpdateHealthBar(); 
        HandleHunger();
        UpdateHungerBar();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        
        Vector3 direction = (transform.right * horizontal + transform.forward * vertical) * currentSpeed;
        Vector3 newPosition = rb.position + direction * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void HandleJump()
    {
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
            return;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && currentSprintValue >= jumpSprintCost)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            currentSprintValue = Mathf.Max(0, currentSprintValue - jumpSprintCost);
            jumpTimer = jumpCooldown;
        }
    }

    void HandleSprint()
    {
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && currentSprintValue > 0;
        targetSprintSpeed = wantsToSprint ? 1f : 0f;
        
        if (wantsToSprint)
        {
            isSprinting = true;
            currentSprintValue = Mathf.Max(0, currentSprintValue - sprintDepletionRate * Time.deltaTime);
            
            if (audioSource != null && sprintSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(sprintSound);
            }
        }
        else
        {
            isSprinting = false;
            if (currentSprintValue < maxSprintValue)
            {
                float regeneration = sprintRegenerationRate * Time.deltaTime;
                currentSprintValue = Mathf.Clamp(currentSprintValue + regeneration, 0f, maxSprintValue);
            }
        }
    }

    void UpdateSprintBar()
    {
        if (sprintBarImage == null) {
            Debug.LogError("SprintBarImage not assigned to Player script!");
            return;
        } 

        // Smooth sprint bar transition
        float targetFill = currentSprintValue / maxSprintValue;
        sprintBarImage.fillAmount = Mathf.Lerp(sprintBarImage.fillAmount, targetFill, Time.deltaTime * sprintTransitionSpeed);
    }

    private void UpdateHealthBar()
    {
        if (!healthBarImage || !playerHealth) return;
        
        targetHealthValue = playerHealth.GetCurrentHealth();
        displayedHealth = Mathf.Lerp(displayedHealth, targetHealthValue, Time.deltaTime * healthBarTransitionSpeed);
        
        float currentMaxHealth = playerHealth.GetMaxHealth();
        float normalizedHealth = displayedHealth / currentMaxHealth;
        
        healthBarImage.fillAmount = normalizedHealth;
        healthText.text = $"{Mathf.Round(displayedHealth)}/{currentMaxHealth}";
    }

    void HandleHunger()
    {
        currentHunger = Mathf.Max(0, currentHunger - hungerDepletionRate * Time.deltaTime);
        
        if (currentHunger <= 0)
        {
            playerHealth.TakeDamage(hungerDamageRate * Time.deltaTime);
        }
    }

    void UpdateHungerBar()
    {
        if (hungerBarImage == null) return;

        float normalizedHunger = currentHunger / maxHunger;
        hungerBarImage.fillAmount = Mathf.Lerp(
            hungerBarImage.fillAmount,
            normalizedHunger,
            Time.deltaTime * hungerBarTransitionSpeed
        );
    }

    public void ConsumeFood(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0, maxHunger);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Check contact normal to ensure we're landing on top of the ground
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }
}