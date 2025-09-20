using UnityEngine;

public class HealthBar2D : MonoBehaviour
{
    [Header("Target")]
    public BaseHealth target;                 // BaseHealth

    [Header("Hierarchy (B seçeneği)")]
    public Transform leftPivot;               // << Boş GO (BG'nin solunda)
    public SpriteRenderer fillSR;             // Kırmızı bar (leftPivot'un child'ı)
    public SpriteRenderer bgSR;               // Opsiyonel arka plan

    [Header("Visuals")]
    public Color fullColor = Color.red;       // Tam can
    public Color emptyColor = Color.black;    // 0 can (kararma)
    public bool smooth = true;
    public float smoothSpeed = 10f;
    public bool autoAlignFill = true;         // Fill'i otomatik yarı genişlik kadar sağa kaydır

    // Dahili
    float shownT = 1f;                        // ekranda gösterilen değer (0..1)

    void Awake()
    {
        if (!target || !leftPivot || !fillSR)
        {
            Debug.LogError("[HealthBar2D] target / leftPivot / fillSR eksik!");
            enabled = false; return;
        }
    }

    void Start()
    {
        // Fill merkez pivotluysa, sol kenarını leftPivot'a hizalayalım:
        if (autoAlignFill) AlignFillHalfWidthRight();

        // Başlangıç görünümü
        shownT = Mathf.Clamp01((float)target.currentHP / target.maxHP);
        Apply(shownT, instant: true);
    }

    void Update()
    {
        float t = Mathf.Clamp01((float)target.currentHP / target.maxHP);

        if (smooth)
            shownT = Mathf.Lerp(shownT, t, 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime));
        else
            shownT = t;

        Apply(shownT, instant: false);
    }

    void Apply(float t, bool instant)
    {
        // 1) Sol sabit kalsın: sadece LEFT PIVOT'un X ölçeğini değiştir
        //    (0'a tam gitmesin diye min bir değer bırakıyoruz, çökme/kapanma olmasın)
        float sx = Mathf.Max(0.0001f, t);
        leftPivot.localScale = new Vector3(sx, 1f, 1f);

        // 2) Renk: kırmızı -> siyah (can azaldıkça kararır)
        fillSR.color = Color.Lerp(emptyColor, fullColor, t);
    }

    // Fill'in pivotu CENTER ise soldan sabitlemek için,
    // fill'i yarı genişliği kadar sağa iteriz (localSpace).
    void AlignFillHalfWidthRight()
    {
        // sprite.bounds.size yerel uzayda (ppu/scale etkili); localScale'ı da hesaba kat.
        float widthLocal = fillSR.sprite.bounds.size.x * fillSR.transform.localScale.x;
        var lp = fillSR.transform.localPosition;
        lp.x = widthLocal * 0.5f;   // sol kenarı leftPivot'ta kalacak şekilde
        fillSR.transform.localPosition = lp;
    }
}
