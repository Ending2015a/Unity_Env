using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScript : MonoBehaviour {

	public int gameMode = 0;
	public GameObject optionPanel;
	public SocketServer server;
	public InputField portInput;

	public UnityEngine.UI.Text LogContent;
	public GameObject robot;
	public GameObject orthcam;

	public bool showminimap = true;
	public bool fixedminimap = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F1)) {
			//TODO: pop up option window
			optionPanel.SetActive(!optionPanel.activeSelf);
		}
		if (Input.GetKeyDown (KeyCode.F2)) {
			//TODO: change game mode
			//0 : Robot mode
			//1 : Spectator mode
			//2 : Setting mode
			gameMode++;
			gameMode %= 3;
		}

		List<string> lists = LogWriter.getLogList ();
		LogContent.text = "";
		foreach (string str in lists) {
			LogContent.text = LogContent.text + str; 
		}

	}



	public void OnSaveClick(){
		server.port = int.Parse (portInput.text);
		LogWriter.Log (string.Format ("Server port has been changed to {0}", server.port));
		server.Rebind ();
	}

	public int GameMode{
		get { return gameMode; }
		set { gameMode = value; }
	}

	public void showMinimap(){
		showminimap = !showminimap;
		orthcam.SetActive (showminimap);
	}

	public void fixedMinimap(){
		fixedminimap = !fixedminimap;
		orthcam.GetComponent<MinimapScript>().fixedRotation = fixedminimap;
	}

}
