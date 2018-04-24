using System;
using System.Collections;
using UnityEngine;

public static class TransitionUtil
{
    public static IEnumerator TransitionToAndBack(float timeTo, float timeDelay, float timeBack, Action<float> withFadeValue, Action onReachedTo = null, Action onFinish = null)
	{
		yield return TransitionTo(0, 1, timeTo, withFadeValue);
        yield return new WaitForSeconds(timeDelay);
		onReachedTo();
		yield return TransitionTo(1, 0, timeBack, withFadeValue, onFinish);
	}

	public static IEnumerator TransitionTo(float from, float to, float duration, Action<float> withFadeValue, Action onFinish = null)
	{
		if (duration <= 0)
		{
			duration = .001f;
		}

		float startTime = Time.time;
		float endTime = startTime + duration;

		float lerp = 0;
		while (lerp < 1)
		{
			lerp = Mathf.Clamp01(Mathf.InverseLerp(startTime, endTime, Time.time));
			withFadeValue(Mathf.Lerp(from, to, lerp));
			yield return null;
		}
		if (onFinish != null)
			onFinish();
	}
	public static void RunFadeTo(Color fromColor, Color toColor, float timeTo, Action onFinish)
	{
		GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>("FullScreenFade"));
		GameObject.DontDestroyOnLoad(gameObject);
		FullScreenFade fullScreenFade = gameObject.GetComponent<FullScreenFade>();

		fullScreenFade.image.color = fromColor;

		Action<float> withFadeValue = (lerp) =>
		{
			fullScreenFade.image.color = Color.Lerp(fromColor, toColor, lerp);
		};
		onFinish += () => GameObject.Destroy(gameObject);
		fullScreenFade.StartCoroutine(TransitionUtil.TransitionTo(0,1, timeTo, withFadeValue, onFinish));
	}

    public static void RunFadeToAndBack(Color fromColor, Color toColor, float timeTo, float timeDelay, float timeBack, Action onToReached, Action onFinish)
	{
		GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>("FullScreenFade"));
		GameObject.DontDestroyOnLoad(gameObject);
		FullScreenFade fullScreenFade = gameObject.GetComponent<FullScreenFade>();

		fullScreenFade.image.color = fromColor;

		Action<float> withFadeValue = (lerp) =>
		{
			fullScreenFade.image.color = Color.Lerp(fromColor, toColor, lerp);
		};

		onFinish += () => GameObject.Destroy(gameObject);
        fullScreenFade.StartCoroutine(TransitionUtil.TransitionToAndBack(timeTo, timeDelay, timeBack, withFadeValue, onToReached, onFinish));
	}
}
