using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

// Bas�l� tutuldu�unda belirli aral�klarla i�lem tetikleyen component
public class HoldRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Action onTick;       // Tetiklenecek aksiyon
    private float delay;         // �lk tekrar gecikmesi
    private float rate;          // Tekrar aral���
    private bool holding;        // Bas�l� tutuluyor mu?
    private Coroutine routine;

    // Ba�latma
    public void Init(Action onTick, float delay, float rate)
    {
        this.onTick = onTick;
        this.delay = delay;
        this.rate = rate;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
        onTick?.Invoke(); // �lk tetikleme
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Repeat());
    }

    public void OnPointerUp(PointerEventData eventData) => StopHold();
    public void OnPointerExit(PointerEventData eventData) => StopHold();

    // Bas�l� tutuldu�u s�rece tekrar et
    IEnumerator Repeat()
    {
        yield return new WaitForSeconds(delay);
        while (holding)
        {
            onTick?.Invoke();
            yield return new WaitForSeconds(rate);
        }
    }

    void StopHold()
    {
        holding = false;
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }
}
