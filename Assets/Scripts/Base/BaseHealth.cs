using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BaseHealth : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public int maxHP = 500;      // Baþlangýç caný
    public int currentHP;        // Güncel can

    void Awake() => currentHP = maxHP;

    
    /// Base'e hasar uygular ve yok olma durumunu kontrol eder
    
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("Base HP: " + currentHP);

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Base yok oldu!");

            // Menü sahnesine dönüþ (birkaç saniye gecikmeli)
            StartCoroutine(ReturnToMenuAfterDelay(2f));
        }
    }

   
    /// Belirtilen süre sonra menü sahnesine döner
  
    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
