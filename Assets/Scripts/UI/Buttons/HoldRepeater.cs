using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

// Basýlý tutulduðunda belirli aralýklarla iþlem tetikleyen component
public class HoldRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Action onTick;       // Tetiklenecek aksiyon
    private float delay;         // Ýlk tekrar gecikmesi
    private float rate;          // Tekrar aralýðý
    private bool holding;        // Basýlý tutuluyor mu?
    private Coroutine routine;

    // Baþlatma
    public void Init(Action onTick, float delay, float rate)
    {
        this.onTick = onTick;
        this.delay = delay;
        this.rate = rate;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
        onTick?.Invoke(); // Ýlk tetikleme
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Repeat());
    }

    public void OnPointerUp(PointerEventData eventData) => StopHold();
    public void OnPointerExit(PointerEventData eventData) => StopHold();

    // Basýlý tutulduðu sürece tekrar et
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
