using UnityEngine;

public class ActiveAnimationCounter : MonoBehaviour
{
    void Update()
    {
        int active = 0;
        foreach (var anim in FindObjectsOfType<Animator>())
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) // devam eden animasyon
                active++;
        }
        Debug.Log("Aktif animasyon: " + active);
    }
}
