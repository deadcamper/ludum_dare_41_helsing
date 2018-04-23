using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextScene;
	
    public void LoadNext()
    {
		TransitionUtil.RunFadeToAndBack(
			new Color(0, 0, 0, 0), 
			new Color(0, 0, 0, 1), 
			2, 
            2, 
			2, 
			() => { SceneManager.LoadScene(nextScene); }, 
            () => {}
			);
    }
}
