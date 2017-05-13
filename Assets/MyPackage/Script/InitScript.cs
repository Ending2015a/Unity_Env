using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScript : MonoBehaviour {

	public int gameMode = 0;
	public GameObject optionPanel;
	private SocketServer server;
	public InputField portInput;

	public UnityEngine.UI.Text LogContent;

	public string controller_name;
	public string orthcam_name;
	public string socket_server_object_name;
	private GameObject robot;
	private GameObject orthcam;
	public UnityEngine.UI.Text posText;
	public UnityEngine.UI.Text rotText;

	public bool showminimap = true;
	public bool fixedminimap = false;

	void Awake(){
		robot = GameObject.Find(controller_name);
		if (robot == null) {
			Debug.LogError (string.Format("Cannot find object '{0}'", controller_name));
		}

		orthcam = GameObject.Find (orthcam_name);
		if (orthcam == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", orthcam));
		}

		GameObject server_obj = GameObject.Find (socket_server_object_name);
		if (server_obj == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", orthcam));
		} else {
			server = server_obj.GetComponent<SocketServer> ();
		}
	}

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

		posText.text = "Pos: " + robot.transform.localPosition;
		rotText.text = "Rot: " + robot.transform.localEulerAngles;
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

	public void OnToggle(int index){
		robot.GetComponent<UnityStandardAssets.Characters.FirstPerson.ControllerScript> ().OnChangeMode (index);
	}

}
