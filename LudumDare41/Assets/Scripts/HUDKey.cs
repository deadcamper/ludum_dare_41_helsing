using UnityEngine;

public class HUDKey : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.1f);
	}
}
