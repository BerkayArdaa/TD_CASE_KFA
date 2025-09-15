using UnityEngine;

[RequireComponent(typeof(BaseHealth))]
public class BaseDamageReceiver : MonoBehaviour
{
    public int damagePerEnemy = 20;   // Her vuruþtaki hasar
    public float damageInterval = 2f; // Kaç saniyede bir hasar

    private BaseHealth baseHealth;
    private float lastDamageTime;

    void Awake()
    {
        baseHealth = GetComponent<BaseHealth>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Belirtilen süre geçtiyse hasar uygula
            if (Time.time >= lastDamageTime + damageInterval)
            {
                baseHealth.TakeDamage(damagePerEnemy);
                lastDamageTime = Time.time;
                Debug.Log("Enemy temas ediyor, base caný: " + baseHealth.currentHP);
            }
        }
    }
}
