using UnityEngine;

public class DamageBox : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool continuousDamage = false;
    [SerializeField] private float damageTickRate = 1f;
    [SerializeField] private ParticleSystem hitEffect;

    private float nextDamageTime;

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (continuousDamage && Time.time >= nextDamageTime)
        {
            ApplyDamage(other);
            nextDamageTime = Time.time + damageTickRate;
        }
    }

    private void ApplyDamage(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            if (hitEffect != null)
            {
                Instantiate(hitEffect, other.transform.position, Quaternion.identity);
            }
        }
    }
}