using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDamaged;

    [Header("I-Frame")]
    [Tooltip("Hasar al�nca ge�ilecek yenilmezlik s�resi (sn).")]
    public float iFrameOnHit = 0.8f;
    [Tooltip("Dirildikten sonra yenilmezlik s�resi (sn).")]
    public float iFrameOnRespawn = 1.5f;
    [Tooltip("Yan�p s�nme aral��� (sn).")]
    public float blinkInterval = 0.1f;
   
    [Tooltip("Normal katman ad� (�r. Player).")]
    public string normalLayerName = "Player";
    [Tooltip("I-frame katman ad� (�r. PlayerIFrame).")]
    public string iFrameLayerName = "PlayerIFrame";

    [Header("Visual Root (opsiyonel)")]
    [Tooltip("Yan�p s�nme i�in renderer�lar� toplayaca��m�z k�k. Bo�sa bu objenin alt� taran�r.")]
    public Transform visualRoot;

    public bool isInvulnerable { get; private set; }

    Renderer[] _renderers;
    int _normalLayer;
    int _iFrameLayer;
    Coroutine _iFrameCo;
    Coroutine _blinkCo;

    void Awake()
    {
        currentHP = maxHP;

        if (visualRoot == null) visualRoot = transform;
        _renderers = visualRoot.GetComponentsInChildren<Renderer>(includeInactive: true);

        _normalLayer = LayerMask.NameToLayer(normalLayerName);
        _iFrameLayer = LayerMask.NameToLayer(iFrameLayerName);
        if (_normalLayer < 0) _normalLayer = gameObject.layer; // emniyet
  
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0) return;
        if (isInvulnerable) return; // I-frame aktifken hasar yok

        int before = currentHP;
        currentHP -= dmg;
        Debug.Log("Player HP: " + currentHP);

        if (currentHP < before)
        {
            onDamaged?.Invoke();
            // Hasar al�nca I-frame'e gir
            StartIFrame(iFrameOnHit);
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            onDeath?.Invoke();
            Destroy(gameObject);
            //TO DO
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    /// <summary>
    /// GameManager vb. taraf�ndan dirilince �a��r.
    /// �ster burada full heal yap, istersen d��ar�da ayarla.
    /// </summary>
    public void OnRespawn(int healTo = -1)
    {
        if (healTo < 0) currentHP = maxHP;
        else currentHP = Mathf.Clamp(healTo, 1, maxHP);

        // Dirili� I-frame
        StartIFrame(iFrameOnRespawn);
    }

    void StartIFrame(float seconds)
    {
        if (_iFrameCo != null) StopCoroutine(_iFrameCo);
        _iFrameCo = StartCoroutine(IFrameRoutine(seconds));
    }

    IEnumerator IFrameRoutine(float seconds)
    {
        isInvulnerable = true;


        // Blink ba�lat
        if (_blinkCo != null) StopCoroutine(_blinkCo);
        _blinkCo = StartCoroutine(BlinkRoutine(seconds));

        yield return new WaitForSeconds(seconds);

        // Blink bitir ve g�r�n�rl��� geri getir
        if (_blinkCo != null) StopCoroutine(_blinkCo);
        SetAllRenderersEnabled(true);

    

        isInvulnerable = false;
        _iFrameCo = null;
    }

    IEnumerator BlinkRoutine(float totalSeconds)
    {
        float end = Time.time + totalSeconds;
        bool visible = true;

        // Renderer.enabled toggling: en garantili yakla��m, shader ba��ms�z �al���r.
        while (Time.time < end)
        {
            visible = !visible;
            SetAllRenderersEnabled(visible);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void SetAllRenderersEnabled(bool enabled)
    {
        if (_renderers == null) return;
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null) _renderers[i].enabled = enabled;
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
