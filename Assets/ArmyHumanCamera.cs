using UnityEngine;
using System.Collections;

public class ArmyHumanCamera : MonoBehaviour {

	public float speed = 100f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Translate(new Vector3(speed,0f,0f));
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Translate(new Vector3(-speed,0f,0f));
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.Translate(new Vector3(0f,-speed,0f));
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.Translate(new Vector3(0f,speed,0f));
		}
		if(Input.GetKey(KeyCode.Z))
		{
			transform.Translate(new Vector3(0f,0f,-speed));
		}
		if(Input.GetKey(KeyCode.X))
		{
			transform.Translate(new Vector3(0f,0f,speed));
		}
	}
}
