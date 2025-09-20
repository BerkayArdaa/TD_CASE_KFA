using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class WaveSelector : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button btnUp;
    [SerializeField] private Button btnDown;
    [SerializeField] private TMP_Text valueText;

    [Header("Ayarlar")]
    [SerializeField] private int minWave = 1;
    [SerializeField] private int maxWave = 200;
    [SerializeField] private int step = 1;
    [SerializeField] private int startValue = 1;
    [SerializeField] private bool zeroPad = false; // 01, 02 gibi

    [Header("Tekrarlý Basma (Tutarken)")]
    [SerializeField] private bool holdRepeat = true;
    [SerializeField] private float holdStartDelay = 0.35f;
    [SerializeField] private float holdRepeatRate = 0.06f;

    [System.Serializable] public class IntEvent : UnityEvent<int> { }
    public IntEvent onValueChanged;

    public int Value { get; private set; }

    void Awake()
    {
        Value = Mathf.Clamp(startValue, minWave, maxWave);
        RefreshUI();

        btnUp.onClick.AddListener(() => Change(+step));
        btnDown.onClick.AddListener(() => Change(-step));

        if (holdRepeat)
        {
            var upHold = btnUp.gameObject.AddComponent<HoldRepeater>();
            upHold.Init(() => Change(+step), holdStartDelay, holdRepeatRate);

            var downHold = btnDown.gameObject.AddComponent<HoldRepeater>();
            downHold.Init(() => Change(-step), holdStartDelay, holdRepeatRate);
        }
    }

    void Update()
    {
        // Klavye
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Change(+step);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Change(-step);

        // Scroll
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
            Change(scroll > 0 ? +step : -step);
    }

    void Change(int delta)
    {
        int old = Value;
        Value = Mathf.Clamp(Value + delta, minWave, maxWave);
        if (Value != old)
        {
            RefreshUI();
            onValueChanged?.Invoke(Value);
        }
    }

    void RefreshUI()
    {
        valueText.text = zeroPad ? Value.ToString("00") : Value.ToString();
        btnDown.interactable = Value > minWave;
        btnUp.interactable = Value < maxWave;
    }
}
