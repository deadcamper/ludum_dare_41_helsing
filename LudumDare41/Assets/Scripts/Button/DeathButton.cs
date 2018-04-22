using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathButton : MonoBehaviour
{
    public Button butt;

	void Start()
	{
        butt.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Inventory.Load(); // reload your last inventory
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload the current scene
    }
}