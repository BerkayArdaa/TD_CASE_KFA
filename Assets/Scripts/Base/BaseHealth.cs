using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

            // Base objesini yok etmeden menüye dön
            StartCoroutine(ReturnToMenuAfterDelay(2f));
        }
    }

    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

      
        SceneManager.LoadScene("Menu");
    }
}
