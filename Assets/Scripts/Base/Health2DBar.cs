using UnityEngine;

public class HealthBar2D : MonoBehaviour
{
    [Header("Hedef")]
    public BaseHealth target;                 // Can kaynağı

    [Header("Hiyerarşi")]
    public Transform leftPivot;               // Sol kenarı sabitleyen pivot
    public SpriteRenderer fillSR;             // Dolu (ön) bar
    public SpriteRenderer bgSR;               // Opsiyonel arka plan

    [Header("Görsel")]
    public Color fullColor = Color.red;       // Tam can rengi
    public Color emptyColor = Color.black;    // 0 can rengi
    public bool smooth = true;                // Yumuşak geçiş
    public float smoothSpeed = 10f;           // Geçiş hızı
    public bool autoAlignFill = true;         // Fill'i sola hizala (center pivot için)

    // Dahili
    float shownT = 1f;                        // Gösterilen oran (0..1)

    void Awake()
    {
        if (!target || !leftPivot || !fillSR)
        {
            Debug.LogError("[HealthBar2D] target / leftPivot / fillSR eksik!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        if (autoAlignFill) AlignFillHalfWidthRight();

        shownT = Mathf.Clamp01((float)target.currentHP / target.maxHP);
        Apply(shownT);
    }

    void Update()
    {
        float t = Mathf.Clamp01((float)target.currentHP / target.maxHP);
        shownT = smooth
            ? Mathf.Lerp(shownT, t, 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime))
            : t;

        Apply(shownT);
    }

    // Bar ölçeği ve rengi uygula
    void Apply(float t)
    {
        // Sol kenar sabit: sadece X ölçeği değişir
        float sx = Mathf.Max(0.0001f, t);
        leftPivot.localScale = new Vector3(sx, 1f, 1f);

        // Can azaldıkça kararır
        fillSR.color = Color.Lerp(emptyColor, fullColor, t);
    }

    // Fill pivotu soldan sabitlemek için yarı genişlik kadar sağa kaydır
    void AlignFillHalfWidthRight()
    {
        if (!fillSR || fillSR.sprite == null) return;

        float widthLocal = fillSR.sprite.bounds.size.x * fillSR.transform.localScale.x;
        var lp = fillSR.transform.localPosition;
        lp.x = widthLocal * 0.5f;
        fillSR.transform.localPosition = lp;
    }
}
