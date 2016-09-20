using UnityEngine;
using System.Collections;

public class PropellerRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		transform.Rotate (0, 0, 3000f*Time.deltaTime);
	}
}
