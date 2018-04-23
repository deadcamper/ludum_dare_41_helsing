using UnityEngine;


public class EpicFinale : MonoBehaviour
{
	void Start()
	{
        PrepForFinale();
    }


    public void PrepForFinale()
    {
        Songs snd = FindObjectOfType<Songs>();

        if(snd != null)
            snd.SetSong(null);
    }

    public void TriggerFinale()
    {
        Enemy.BYPASS_TURN_DURATION = true; //SPEED UP

        Songs snd = FindObjectOfType<Songs>();

        if (snd != null)
            snd.SetSong(snd.finaleSong);
    }
}