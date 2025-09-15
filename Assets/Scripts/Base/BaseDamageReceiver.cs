using UnityEngine;

[RequireComponent(typeof(BaseHealth))]
public class BaseDamageReceiver : MonoBehaviour
{
    public int damagePerEnemy = 20;   // Her vuru�taki hasar
    public float damageInterval = 2f; // Ka� saniyede bir hasar

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
            // Belirtilen s�re ge�tiyse hasar uygula
            if (Time.time >= lastDamageTime + damageInterval)
            {
                baseHealth.TakeDamage(damagePerEnemy);
                lastDamageTime = Time.time;
                Debug.Log("Enemy temas ediyor, base can�: " + baseHealth.currentHP);
            }
        }
    }
}
