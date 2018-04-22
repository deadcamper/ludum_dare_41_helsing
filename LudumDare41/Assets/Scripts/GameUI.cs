using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Game game;

    public GameObject gameOverScreen;
    public GameObject gamePausedScreen;
    public GameObject inGameMessage;
    public Text inGameMessageText;

    public enum UIState
    {
        PLAYING,
        PAUSED,
        GAMEOVER,
        GAMEWIN
    }

    [HideInInspector]
    public UIState currentState;

	void Start()
	{
        UpdateState(UIState.PLAYING);
    }	

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIState nextState = currentState == UIState.PLAYING ? UIState.PAUSED : UIState.PLAYING;
            UpdateState(nextState);
        }
	}

    public static void DisplayMessage(string message)
    {
        GameUI gameUI = FindObjectOfType<GameUI>();
        gameUI.inGameMessageText.text = message;
        gameUI.inGameMessage.SetActive(true);
        gameUI.HideInGameMessage();
    }

    public void HideInGameMessage()
    {
        StartCoroutine(DelayAndHide());
    }

    private IEnumerator DelayAndHide()
    {
        yield return new WaitForSeconds(2.0f);
        inGameMessage.SetActive(false);
    }

    public void UpdateState(UIState newState)
    {
        if (currentState == UIState.GAMEOVER || currentState == UIState.GAMEWIN)
        {
            //Do nothing
            return;
        }

        if (newState == UIState.PAUSED)
        {
            gamePausedScreen.SetActive(true);
            gameOverScreen.SetActive(false);
            currentState = UIState.PAUSED;
        }
        else if (newState == UIState.PLAYING)
        {
            gamePausedScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            currentState = UIState.PLAYING;
        }
        else if (newState == UIState.GAMEOVER)
        {
            gamePausedScreen.SetActive(false);
            gameOverScreen.SetActive(true);
            currentState = UIState.GAMEOVER;
        }
        //TODO Game won
    }
}