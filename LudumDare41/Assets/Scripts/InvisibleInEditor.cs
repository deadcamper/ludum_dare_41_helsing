using UnityEngine;

[ExecuteInEditMode]
public class InvisibleInEditor : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Canvas>().enabled = !Application.isEditor || Application.isPlaying;
    }

    private void OnDisable()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
