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
            int childCount = hud.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                hud.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public static void Show()
    {
        HUD hud = FindObjectOfType<HUD>();
        if (hud)
        {
            int childCount = hud.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                hud.transform.GetChild(i).gameObject.SetActive(true);
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

        int val = 0, metalVal = 0;
        player.Inventory.Items.TryGetValue(ItemType.Stake, out val);

        player.Inventory.Items.TryGetValue(ItemType.MetalStake, out metalVal);
        stakeText.text = (metalVal > 0) ? "METAL" : val.ToString();

        player.Inventory.Items.TryGetValue(ItemType.SilverBullet, out val);
        bulletsText.text = val.ToString();
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