using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BiteEffectAutoDestroy : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // E�er Particle System varsa, animasyon beklemek yerine
        // bitince kendini yok etmesini de garantiye alal�m.
        if (TryGetComponent<ParticleSystem>(out var ps))
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy; // PS bitince objeyi yok et
        }
    }

    void OnEnable()
    {
        // Animator�l� klibin biti�ine g�re yok et
        StartCoroutine(DestroyAfterCurrentState());
    }

    private IEnumerator DestroyAfterCurrentState()
    {
        // Bir frame bekle ki animator state tam otursun
        yield return null;

        if (animator == null) yield break;

        // Layer 0��n mevcut state�inin s�resi (speed�i hesaba kat)
        var info = animator.GetCurrentAnimatorStateInfo(0);
        float seconds = info.length / Mathf.Max(animator.speed, 0.0001f);

        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }

    // �stersen animasyonun son frame�ine Animation Event koyup bunu �a��rabilirsin:
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
