using UnityEngine;
using UnityEngine.UI;

public class ResumeButton : MonoBehaviour
{
    public GameUI gameUI;
    public Button butt;

	void Start()
	{
        butt.onClick.AddListener(OnResume);
	}

    void OnResume()
    {
        gameUI.UpdateState(GameUI.UIState.PLAYING);
    }


}