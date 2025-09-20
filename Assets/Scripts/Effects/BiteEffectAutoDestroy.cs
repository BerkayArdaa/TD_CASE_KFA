using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BiteEffectAutoDestroy : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Particle System varsa bitince objeyi yok et
        if (TryGetComponent<ParticleSystem>(out var ps))
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }

    void OnEnable()
    {
        // Animator klibi bitince objeyi yok et
        StartCoroutine(DestroyAfterCurrentState());
    }

    private IEnumerator DestroyAfterCurrentState()
    {
        yield return null; // animator state’in oturmasýný bekle

        if (animator == null) yield break;

        // Aktif animasyonun süresine göre bekle
        var info = animator.GetCurrentAnimatorStateInfo(0);
        float seconds = info.length / Mathf.Max(animator.speed, 0.0001f);

        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

  
    /// Animation Event ile çaðrýlabilir alternatif yok etme metodu
   
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
