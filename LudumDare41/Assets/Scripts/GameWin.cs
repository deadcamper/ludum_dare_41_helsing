using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameWin : MonoBehaviour
{

	public string nextScene;
	public Image blackImage;
	public Text text;
    public Songs songs;

	void Start()
	{
        songs = FindObjectOfType<Songs>();

		GetComponent<Canvas>().enabled = true;
		SetBlackImageOpacity(0);
		SetTextOpacity(0);

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

		DontDestroyOnLoad(gameObject);

		string nxtScene = nextScene;

        yield return new WaitForSeconds(4);

		yield return TransitionUtil.TransitionTo(0, 1, 8, SetBlackImageOpacity);

        HUD.Hide();

		yield return TransitionUtil.TransitionTo(0, 1, 2, SetTextOpacity);

		yield return new WaitForSeconds(2);

        yield return TransitionUtil.TransitionTo(1, 0, 4, FadeMusicAndSetTextOpacity);
        yield return new WaitForSeconds(2);

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

    void FadeMusicAndSetTextOpacity(float opacity)
    {
        songs.finaleSong.volume = opacity;
        SetTextOpacity(opacity);
    }

	void SetBlackImageOpacity(float opacity)
	{
		Color color = blackImage.color;
		color.a = opacity;
		blackImage.color = color;
	}
}
