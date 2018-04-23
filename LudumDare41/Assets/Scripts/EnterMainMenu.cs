using UnityEngine;

namespace FPG
{
	public class EnterMainMenu : MonoBehaviour
	{
		void Start()
		{
            HUD.Hide();
            Inventory.Clear();
            Inventory.AddStartingItems();
        }
	}
}