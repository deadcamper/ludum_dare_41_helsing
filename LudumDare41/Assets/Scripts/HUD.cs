using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text stakeText;
    public Text bulletsText;

    private Player player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        int val = 0;
        player.Inventory.Items.TryGetValue(ItemType.Stake, out val);
        stakeText.text = "Stakes: " + val;

        player.Inventory.Items.TryGetValue(ItemType.SilverBullet, out val);
        bulletsText.text = "Silver Bullets: " + val;
    }
}