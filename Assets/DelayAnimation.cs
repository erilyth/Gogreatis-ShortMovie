using UnityEngine;
using System.Collections;

public class DelayAnimation : MonoBehaviour {

	public float delay;

	// Use this for initialization
	void Start () {
		Invoke("PlayBase", delay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlayBase()
	{
		GetComponent<Animation>().Play();
	}
}
