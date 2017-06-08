using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScript : MonoBehaviour {


	private SocketServer server;
	[Header("Server Settings")]
	public InputField portInput;



	[Header("Robot/Camera Settings")]
	public string controller_name;
	public string fpscam_name;
	public string orthcam_name;
	public string depthcam_name;
	public string spherecam_name;
	public string socket_server_object_name;
	private GameObject robot;
	private GameObject fpscam;
	private GameObject orthcam;
	private GameObject depthcam;
	private GameObject spherecam;
	private Vector3 InitialPosition;
	public UnityEngine.UI.Text posText;
	public UnityEngine.UI.Text rotText;

	[Header("UI Settings")]
	public GameObject optionPanel;
	public UnityEngine.UI.Text LogContent;
	public UnityEngine.UI.InputField timeScaleInput;
	public UnityEngine.UI.Toggle cameraToggle;
	public UnityEngine.UI.Toggle minimapToggle;
	public UnityEngine.UI.Toggle minimapRotateToggle;
	public UnityEngine.UI.Toggle depthMapToggle;
	public UnityEngine.UI.Toggle SphericalToggle;
	public bool showcamera = true;
	public bool showminimap = true;
	public bool fixedminimap = false;
	public bool showdepth = false;
	public bool enablespherical = false;

	void Awake(){
		robot = GameObject.Find(controller_name);
		if (robot == null) {
			Debug.LogError (string.Format("Cannot find object '{0}'", controller_name));
		}

		fpscam = GameObject.Find (fpscam_name);
		if (fpscam == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", fpscam_name));
		}

		orthcam = GameObject.Find (orthcam_name);
		if (orthcam == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", orthcam_name));
		}

		depthcam = GameObject.Find (depthcam_name);
		if (depthcam == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", depthcam_name));
		}

		spherecam = GameObject.Find (spherecam_name);
		if (spherecam == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", spherecam_name));
		}

		GameObject server_obj = GameObject.Find (socket_server_object_name);
		if (server_obj == null) {
			Debug.LogError (string.Format ("Cannot find object '{0}'", socket_server_object_name));
		} else {
			server = server_obj.GetComponent<SocketServer> ();
		}

		InitialPosition = robot.transform.position;
	}

	// Use this for initialization
	void Start () {
		showCamera ();
		showMinimap ();
		fixedMinimap ();
		showDepth ();
		timeScaleInput.text = Time.timeScale.ToString();
		portInput.text = server.port.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F1)) {
			//TODO: pop up option window
			optionPanel.SetActive(!optionPanel.activeSelf);
		}

		List<string> lists = LogWriter.getLogList ();
		LogContent.text = "";
		foreach (string str in lists) {
			LogContent.text = LogContent.text + str; 
		}

		posText.text = "Pos: " + robot.transform.localPosition;
		rotText.text = "Rot: " + robot.transform.localEulerAngles;

	}



	public void OnRestartServer(){
		int portnum;

		if (!int.TryParse(portInput.text, out portnum) || portnum < 0 || portnum > 65535) {
			LogWriter.Error (string.Format("Invalid Port Number {0}", portInput.text));
			return;
		}
		server.port = int.Parse (portInput.text);
		LogWriter.Log (string.Format ("Server port has been changed to {0}", server.port));
		server.Rebind ();
	}

	public int GameMode{
		get { return 0; }
		set { }
	}

	public void showMinimap(){
		showminimap = minimapToggle.isOn;
		orthcam.SetActive (showminimap);
	}

	public void fixedMinimap(){
		fixedminimap = minimapRotateToggle.isOn;
		orthcam.GetComponent<MinimapScript>().fixedRotation = fixedminimap;
	}

	public void OnToggle(int index){
		robot.GetComponent<UnityStandardAssets.Characters.FirstPerson.ControllerScript> ().OnChangeMode (index);
	}

	public void showDepth(){
		showdepth = depthMapToggle.isOn;
		depthcam.GetComponent<ScreenshotScript> ().renderToScreen (showdepth);
	}

	public void showSpherical(){
		enablespherical = SphericalToggle.isOn;
		spherecam.GetComponent<ScreenshotScript> ().renderToScreen (enablespherical);
		/*fpscam.GetComponent<SphericalImageCam_Free> ().enablespherical = enablespherical;*/
	}

	public void showCamera(){
		showcamera = cameraToggle.isOn;
		fpscam.GetComponent<ScreenshotScript> ().renderToScreen (showcamera);
	}

	public float TimeScale{
		get{ 
			return Time.timeScale;
		}
		set{
			if (value < 0)
				value = -value;
			timeScaleInput.text = value.ToString ();
			Time.timeScale = value;
			LogWriter.Log ("The TimeScale has been changed to " + value.ToString ());
		}
	}

	public void OnTimeScale(){
		float scale = 1;
		if (!float.TryParse (timeScaleInput.text, out scale)) {
			LogWriter.Error (string.Format ("Invalid TimeScale: {0}", timeScaleInput.text));
			return;
		}
		TimeScale = scale;
	}

	public void OnRandPos(){
		Vector3 pos = robot.GetComponent<RandPosGenerator> ().getRandPos ();
		robot.transform.position = pos;
		LogWriter.Log (string.Format ("The robot has been transport to position {0}", pos.ToString ()));
	}

}
