using UnityEngine;
using System.Collections;

public class globalhuman : MonoBehaviour {

	public float delay;

	// Use this for initialization
	void Start () {
		Invoke("startAnimation", delay);
	}

	// Update is called once per frame
	void Update () {

	}

	void startAnimation()
	{
		GetComponent<Animation>().Play();
	}
}
