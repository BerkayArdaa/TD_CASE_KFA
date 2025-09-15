// PlayerSlow.cs  (Player'a ekle)
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSlow : MonoBehaviour
{
    [Tooltip("Hareket scriptinizde kullanýlan temel hýz (örn. PlayerMovement.moveSpeed)")]
    public float baseMoveSpeed = 6f;

    [Tooltip("Hareket eden script (örn. PlayerMovement). Oradan hýza ulaþacaðýz.")]
    public PlayerMovement movement;   // kendi hareket scriptinin sýnýf adý

    float currentMultiplier = 1f;
    Coroutine slowCo;

    void Reset() { movement = GetComponent<PlayerMovement>(); }

    void Start()
    {
        if (!movement) movement = GetComponent<PlayerMovement>();
        if (movement) movement.moveSpeed = baseMoveSpeed;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, 0.1f, 1f); // 0.5 = %50 hýz
        if (slowCo != null) StopCoroutine(slowCo);
        slowCo = StartCoroutine(SlowRoutine(multiplier, duration));
    }

    IEnumerator SlowRoutine(float m, float dur)
    {
        currentMultiplier = m;
        UpdateSpeed();
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            yield return null;
        }
        currentMultiplier = 1f;
        UpdateSpeed();
        slowCo = null;
    }

    void UpdateSpeed()
    {
        if (movement)
            movement.moveSpeed = baseMoveSpeed * currentMultiplier;
    }
}
