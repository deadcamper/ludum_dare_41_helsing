using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button butt;

	void Start()
	{
        butt.onClick.AddListener(OnQuitClick);
	}

    public void OnQuitClick()
    {
        Application.Quit();
    }
}