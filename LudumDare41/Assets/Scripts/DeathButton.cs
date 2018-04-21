using UnityEngine;
using UnityEngine.UI;

public class DeathButton : MonoBehaviour
{
    public Button butt;
    public Text buttText;

    private int index = -1;
    private string[] WORD_SOUP = new string[] { "Bloat", "Decompose", "Rot", "Stare", "Do Nothing", "Smell Bad" };

	void Start()
	{
        butt.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        int nextIndex;

        do {
            nextIndex = Random.Range(0, WORD_SOUP.Length);
        } while (index == nextIndex);

        buttText.text = WORD_SOUP[nextIndex];
        index = nextIndex;
    }
}