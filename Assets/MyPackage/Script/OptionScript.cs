using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionScript : MonoBehaviour {

	public UnityEngine.UI.Text postext;
	public UnityEngine.UI.Text rottext;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		postext.text = "Pos: " + this.transform.localPosition;
		rottext.text = "Rot: " + this.transform.localEulerAngles;
	}
}
