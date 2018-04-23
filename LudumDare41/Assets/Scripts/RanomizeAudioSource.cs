using UnityEngine;

public class RanomizeAudioSource : MonoBehaviour
{
    public float min = 0.9f;
    public float max = 1.4f;

    // Use this for initialization
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.pitch = min + Random.value * (max - min);
    }
}
