using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Events")]
    public UnityEvent onDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        currentHP -= dmg;
        Debug.Log("Player HP: " + currentHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
            onDeath?.Invoke();
            Destroy(gameObject);
            // TODO: respawn veya game over
        }
    }

    

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }
}
