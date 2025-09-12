using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 30;
    int hp;

    void Awake() { hp = maxHP; }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log(hp);  
        if (hp <= 0) Die();
    }

    void Die()
    {
        // TODO: death efekt / pool
        Destroy(gameObject);
    }
}
