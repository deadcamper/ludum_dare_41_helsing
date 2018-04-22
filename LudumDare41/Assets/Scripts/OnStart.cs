using UnityEngine;
using UnityEngine.SceneManagement;

public class OnStart : MonoBehaviour 
{
	void Awake() 
    {
        DontDestroyOnLoad(gameObject);

        HUD.Hide();
        SceneManager.LoadScene("MainMenu");
	}
}
