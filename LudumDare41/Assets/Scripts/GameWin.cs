using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWin : MonoBehaviour
{

	public string nextScene;
	public Image blackImage;
	public Text text;


	void Start()
	{
		GetComponent<Canvas>().enabled = true;
		SetBlackImageOpacity(0);
		SetTextOpacity(0);

		DontDestroyOnLoad(gameObject);
		StartCoroutine(Run());
	}

	IEnumerator Run()
	{
		while (!Inventory.GetInstance().HasItem(ItemType.MetalStake))
			yield return null;

		bool allEnemiesDead = false;
		while (!allEnemiesDead)
		{
			yield return null;
			allEnemiesDead = !FindObjectsOfType<Enemy>().Any(e => !e.isDead());
		}
		string nxtScene = nextScene;

		yield return TransitionUtil.TransitionTo(0,1,2, SetBlackImageOpacity);
		yield return TransitionUtil.TransitionTo(0, 1, 2, SetTextOpacity);

		yield return new WaitForSeconds(2);

		yield return TransitionUtil.TransitionTo(1, 0, 2, SetTextOpacity);

		SceneManager.LoadScene(nxtScene);
		yield return TransitionUtil.TransitionTo(1, 0, 2, SetBlackImageOpacity);
		Destroy(gameObject);
	}

	void SetTextOpacity(float opacity)
	{
		Color color = text.color;
		color.a = opacity;
		text.color = color;
	}
	void SetBlackImageOpacity(float opacity)
	{
		Color color = blackImage.color;
		color.a = opacity;
		blackImage.color = color;
	}
}
