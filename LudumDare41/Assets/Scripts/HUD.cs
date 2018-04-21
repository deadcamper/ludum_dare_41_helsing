using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text stakeText;
    public Text bulletsText;

    private Player player;

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            player = FindObjectOfType<Player>();

        int val = 0;
        player.Inventory.Items.TryGetValue(ItemType.Stake, out val);
        stakeText.text = "Stakes: " + val;

        player.Inventory.Items.TryGetValue(ItemType.SilverBullet, out val);
        bulletsText.text = "Silver Bullets: " + val;
    }
}