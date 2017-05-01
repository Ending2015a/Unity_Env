using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour {

	public bool fixedRotation = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!fixedRotation) {
			this.transform.localRotation = Quaternion.Euler (this.transform.localRotation.eulerAngles.x, 0, 0);
		} else {
			this.transform.rotation = Quaternion.Euler (this.transform.rotation.eulerAngles.x, 0, 0);
		}	
	}

	public void FixedRotation(bool fx){
	}
}
