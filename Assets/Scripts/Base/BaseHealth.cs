using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public int maxHP = 500;
    public int currentHP;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("BASE HP: " + currentHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
       
            Debug.Log("BASE DESTROYED!");
            Destroy(gameObject);
        }
    }
}
