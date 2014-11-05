using UnityEngine;
using System.Collections;

public class DrawGui : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnGUI() {
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "This is a title");
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "This is a title");
	}
	
}
