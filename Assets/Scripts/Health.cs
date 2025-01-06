using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public delegate void HealthEvent();
    public event HealthEvent OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    // if the object is not the player, return -1
    public float GetCurrentHealth()
    {
        if (IsPlayer())
            return currentHealth;
        return -1;
    }

    // if the object is not the player, return -1
    public float GetMaxHealth()
    {
        if (IsPlayer())
            return maxHealth;
        return -1;
    }

    public bool IsPlayer()
    {
        return gameObject.CompareTag("Player");
    }
}