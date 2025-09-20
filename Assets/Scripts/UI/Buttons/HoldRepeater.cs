using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class HoldRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Action onTick;
    private float delay, rate;
    private bool holding;
    private Coroutine routine;

    public void Init(Action onTick, float delay, float rate)
    {
        this.onTick = onTick;
        this.delay = delay;
        this.rate = rate;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
        onTick?.Invoke();
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Repeat());
    }

    public void OnPointerUp(PointerEventData eventData) => StopHold();
    public void OnPointerExit(PointerEventData eventData) => StopHold();

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
