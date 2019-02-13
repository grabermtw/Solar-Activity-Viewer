using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looker : MonoBehaviour {

    public Transform myCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(myCamera);
	}
}
