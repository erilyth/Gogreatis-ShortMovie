using UnityEngine;
using System.Collections;

public class AnimationOrb : MonoBehaviour {

	public GameObject cyclone;

	// Use this for initialization
	void Start () {
		Invoke("CreateVictory", 12.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateVictory()
	{
		Instantiate(cyclone, new Vector3(4f, 1.5f, 10f), Quaternion.identity);
	}
}
