using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text stakeText;
    public Text bulletsText;
    public Button shootButton;

    private Player player;
    public Image keyImageSource;
    private List<Image> keyImages = new List<Image>();

    public AudioSource audioSource;
    public bool hidden = false;

    private void Awake()
    {
        Inventory.GetInstance().onItemChange += OnItemChange;
        shootButton.onClick.AddListener(OnClickShoot);
    }

    public static void Hide()
    {
        HUD hud = FindObjectOfType<HUD>();
        if (hud)
        {
            hud.hidden = true;
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
            hud.hidden = false;
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
        }

        int val = 0, metalVal = 0;
        Inventory.GetInstance().Items.TryGetValue(ItemType.Stake, out val);

        Inventory.GetInstance().Items.TryGetValue(ItemType.MetalStake, out metalVal);
        stakeText.text = (metalVal > 0) ? "infinite" : val.ToString();

        Inventory.GetInstance().Items.TryGetValue(ItemType.SilverBullet, out val);
        bulletsText.text = val.ToString();
        // only show when you have not zero bullets
        shootButton.gameObject.SetActive(!hidden && val > 0);
    }

    private void OnClickShoot()
    {
        player.AttemptShoot();
    }

    private void OnItemChange(ItemType itemType, int gainedQty, int totalQty)
    {
        // keys are a special case!
        switch (itemType)
        {
            case ItemType.Key: // TODO as we add other key types, add them here (KeyGreen, KeyBlue, etc)
                if (gainedQty > 0)
                {
                    if (audioSource)
                        audioSource.Play();
                }

                //Do visual correction for keys!
                while (totalQty > keyImages.Count)
                {
                    keyImages.Add(Instantiate(keyImageSource, transform));
                }
                while (totalQty < keyImages.Count)
                {
                    Image img = keyImages[keyImages.Count - 1];
                    Destroy(img.gameObject);
                    keyImages.RemoveAt(keyImages.Count - 1);
                }
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}