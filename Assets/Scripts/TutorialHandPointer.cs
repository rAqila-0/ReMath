using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandPointer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;

    [Header("Animation")]
    [SerializeField] private float moveTime = 1f;

    [SerializeField] private float waitTime = 0.5f;

    [SerializeField]
    private bool playOnEnable = false;

    RectTransform rect;
    CanvasGroup canvasGroup;

    Coroutine routine;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if(canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        Stop();

        routine = StartCoroutine(Animate());
    }

    public void Stop()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    IEnumerator Animate()
    {
        while(true)
        {
            rect.position = startPoint.position;

            canvasGroup.alpha = 0;

            yield return Fade(0,1,0.2f);

            float t = 0;

            while(t < moveTime)
            {
                t += Time.deltaTime;

                float lerp = t / moveTime;

                rect.position = Vector3.Lerp(
                    startPoint.position,
                    endPoint.position,
                    lerp);

                float scale =
                    Mathf.Lerp(1f,0.9f,lerp);

                rect.localScale =
                    Vector3.one * scale;

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            yield return Fade(1,0,0.2f);

            rect.localScale = Vector3.one;

            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator Fade(float from,float to,float duration)
    {
        float t = 0;

        while(t < duration)
        {
            t += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(from,to,t/duration);

            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void SetTarget(RectTransform start, RectTransform end)
    {
        startPoint = start;
        endPoint = end;

        Play();
    }
}