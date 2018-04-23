using UnityEngine;

namespace FPG
{
	public class EnterMainMenu : MonoBehaviour
	{
		void Start()
		{
            Enemy.BYPASS_TURN_DURATION = false;
            HUD.Hide();
            Inventory.Clear();
            Inventory.AddStartingItems();

            Songs sng = FindObjectOfType<Songs>();

            if (sng != null)
                sng.SetSong(sng.normalSong);
        }
	}
}