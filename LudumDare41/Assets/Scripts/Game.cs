using UnityEngine;

public class Game : MonoBehaviour
{
    public GameUI ui;

    public Player player;
    public TurnManager turns;

    private bool gameRunning = true;

	void Start()
	{
		if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (turns == null)
        {
            turns = FindObjectOfType<TurnManager>();
        }
	}	

	void Update()
	{
		if (gameRunning && player.isDead())
        {
            gameRunning = false;
            turns.ShouldRunTurns = false;
            ui.UpdateState(GameUI.UIState.GAMEOVER);
        }
	}

    public bool IsPaused()
    {
        return ui.currentState == GameUI.UIState.PAUSED;
    }
}