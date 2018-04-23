using System.Collections;
using UnityEngine;

public class Songs : MonoBehaviour
{
    public AudioSource normalSong;
    public AudioSource finaleSong;

    private AudioSource currentSong;

    public void SetSong(AudioSource nextSong)
    {
        if (currentSong != null)
            StartCoroutine(FadeOut(currentSong, 0.5f));

        if (nextSong != null)
        {
            nextSong.Play();
            currentSong = nextSong;
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}