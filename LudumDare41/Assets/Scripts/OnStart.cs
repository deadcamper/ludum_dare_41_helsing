using UnityEngine;
using UnityEngine.SceneManagement;

public class OnStart : MonoBehaviour 
{
    public string startScene = "MainMenu";

	void Awake() 
    {
        DontDestroyOnLoad(gameObject);

        HUD.Hide();
        SceneManager.LoadScene(startScene);
	}
}
