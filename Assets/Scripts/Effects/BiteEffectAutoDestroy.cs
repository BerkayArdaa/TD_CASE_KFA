using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BiteEffectAutoDestroy : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Eðer Particle System varsa, animasyon beklemek yerine
        // bitince kendini yok etmesini de garantiye alalým.
        if (TryGetComponent<ParticleSystem>(out var ps))
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy; // PS bitince objeyi yok et
        }
    }

    void OnEnable()
    {
        // Animator’lý klibin bitiþine göre yok et
        StartCoroutine(DestroyAfterCurrentState());
    }

    private IEnumerator DestroyAfterCurrentState()
    {
        // Bir frame bekle ki animator state tam otursun
        yield return null;

        if (animator == null) yield break;

        // Layer 0’ýn mevcut state’inin süresi (speed’i hesaba kat)
        var info = animator.GetCurrentAnimatorStateInfo(0);
        float seconds = info.length / Mathf.Max(animator.speed, 0.0001f);

        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }

    // Ýstersen animasyonun son frame’ine Animation Event koyup bunu çaðýrabilirsin:
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
