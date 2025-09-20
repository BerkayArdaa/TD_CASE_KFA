using UnityEngine;

[RequireComponent(typeof(BaseHealth))]
public class BaseDamageReceiver : MonoBehaviour
{
    [Header("Hasar Ayarlar�")]
    public int damagePerEnemy = 20;      // Her temas ba��na verilecek hasar
    public float damageInterval = 2f;    // Hasar tekrar s�resi (saniye)

    private BaseHealth baseHealth;
    private float lastDamageTime;

    void Awake()
    {
        baseHealth = GetComponent<BaseHealth>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // S�re dolduysa base'e hasar uygula
            if (Time.time >= lastDamageTime + damageInterval)
            {
                baseHealth.TakeDamage(damagePerEnemy);
                lastDamageTime = Time.time;

                Debug.Log("Enemy base'e temas etti, kalan can: " + baseHealth.currentHP);
            }
        }
    }
}
