using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BaseHealth : MonoBehaviour
{
    [Header("Can Ayarlar�")]
    public int maxHP = 500;      // Ba�lang�� can�
    public int currentHP;        // G�ncel can

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

            // Men� sahnesine d�n�� (birka� saniye gecikmeli)
            StartCoroutine(ReturnToMenuAfterDelay(2f));
        }
    }

   
    /// Belirtilen s�re sonra men� sahnesine d�ner
  
    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}
