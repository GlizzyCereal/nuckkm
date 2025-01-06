using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private float hungerRestoreAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ConsumeFood(hungerRestoreAmount);
                Destroy(gameObject);
            }
        }
    }
}