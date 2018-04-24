using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextScene;

	bool loading = false;

    public void LoadNext()
    {
		if (!loading)
		{
			loading = true;

			TransitionUtil.RunFadeToAndBack(
				new Color(0, 0, 0, 0),
				new Color(0, 0, 0, 1),
				4,
				1,
				2,
				LoadNextLvl,
				() => { }
				);
		}
    }
	private void LoadNextLvl()
	{
		SceneManager.LoadScene(nextScene);
	}
}
