using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Juice")]
    public float hoverScale = 1.08f;
    public float pressedScale = 0.90f;
    public float punchScale = 1.16f;
    public float snappiness = 18f;

    private Vector3 baseScale;
    private Vector3 targetScale;
    private bool isHovered;
    private bool isPressed;
    private Coroutine punchRoutine;

    private void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    private void OnEnable()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
        isHovered = false;
        isPressed = false;
    }

    private void Update()
    {
        if (punchRoutine == null)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.unscaledDeltaTime * snappiness
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        RefreshTargetScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        RefreshTargetScale();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        RefreshTargetScale();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        RefreshTargetScale();
        TriggerPunch();
    }

    public void TriggerPunch()
    {
        if (!gameObject.activeInHierarchy) return;
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchCount());
    }

    private IEnumerator PunchCount()
    {
        Vector3 peak = baseScale * punchScale;
        float t = 0f;
        float duration = 0.11f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, peak, t / duration);
            yield return null;
        }

        RefreshTargetScale();
        punchRoutine = null;
    }

    private void RefreshTargetScale()
    {
        if (isPressed)
            targetScale = baseScale * pressedScale;
        else if (isHovered)
            targetScale = baseScale * hoverScale;
        else
            targetScale = baseScale;
    }
}