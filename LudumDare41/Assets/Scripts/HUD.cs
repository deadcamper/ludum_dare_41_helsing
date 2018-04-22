using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text stakeText;
    public Text bulletsText;

    private Player player;
    public Image keyImageSource;
    private Dictionary<ItemType, Image> keyImages = new Dictionary<ItemType, Image>();

    public AudioSource audioSource;

    public static void Hide()
    {
        HUD hud = FindObjectOfType<HUD>();
        if (hud)
        {
            Text[] displays = hud.GetComponentsInChildren<Text>();
            foreach (Text display in displays)
            {
                display.enabled = false;
            }
        }
    }

    public static void Show()
    {
        HUD hud = FindObjectOfType<HUD>();
        if (hud)
        {
            Text[] displays = hud.GetComponentsInChildren<Text>();
            foreach (Text display in displays)
            {
                display.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            player.Inventory.onItemChange += OnItemChange;
        }

        int val = 0;
        player.Inventory.Items.TryGetValue(ItemType.Stake, out val);
        stakeText.text = "Stakes: " + val;

        player.Inventory.Items.TryGetValue(ItemType.SilverBullet, out val);
        bulletsText.text = "Silver Bullets: " + val;
    }

    private void OnItemChange(ItemType itemType, int qty)
    {
        // keys are a special case!
        switch (itemType)
        {
            case ItemType.Key: // TODO as we add other key types, add them here (KeyGreen, KeyBlue, etc)
                if (qty > 0)
                {
                    audioSource.Play();
                    keyImages.Add(itemType, Instantiate(keyImageSource, transform));
                }
                else
                {
                    Destroy(keyImages[itemType]);
                    keyImages.Remove(itemType);
                }
                break;
        }
    }
}