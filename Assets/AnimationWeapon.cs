using UnityEngine;
using System.Collections;

public class AnimationWeapon : MonoBehaviour {

	public GameObject spark;

	// Use this for initialization
	void Start () {
		Invoke("Spark", 1.5f);
	}

	// Update is called once per frame
	void Update () {

	}

	void Spark()
	{
		Instantiate(spark, new Vector3(6f, 3.5f, 13.8f), Quaternion.identity);
	}
}
