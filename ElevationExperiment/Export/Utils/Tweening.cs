// credit to ChatGPT 3.5 for this quick replacement of DG.Tweening
using System;
using UnityEngine;

public class TweeningManager : MonoBehaviour
{
    private static TweeningManager instance;
    public static TweeningManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Tween a value over time, continuously acting on the current value using a lambda function
    public void TweenValue(float startValue, Func<float, float> updateAction, float duration)
    {
        // Create a new tweener and start tweening
        Tweener newTweener = new Tweener(startValue, updateAction, duration);
        newTweener.StartTween();
    }
}

public class Tweener
{
    private float startValue;
    private Func<float, float> updateAction;
    private float duration;
    private float elapsed;

    private bool isTweening;
    private float startTime;
    private float currentValue;
    private float endValue;

    public Tweener(float startValue, Func<float, float> updateAction, float duration)
    {
        this.startValue = startValue;
        this.updateAction = updateAction;
        this.duration = duration;
    }

    public void StartTween()
    {
        isTweening = true;
        startTime = Time.time;
        currentValue = startValue;
        endValue = updateAction(startValue);
    }

    public void StopTween()
    {
        isTweening = false;
    }

    private void Update()
    {
        if (isTweening)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            currentValue = Mathf.Lerp(startValue, endValue, t);
            updateAction(currentValue);

            if (t >= 1f || Mathf.Approximately(currentValue, endValue))
            {
                updateAction(endValue);
                isTweening = false;
            }
        }
    }
}
