using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommandParser : MonoBehaviour {

	public GameObject FirstCamera;
	public GameObject ThirdCamera;
	public GameObject DepthCamera;
	public GameObject SphereCamera;

	private SocketServer server;
	private ScreenshotScript sssfc;
	private ScreenshotScript ssstc;
	private ScreenshotScript sssdc;
	private ScreenshotScript ssssc;

	private InitScript optionPanel;


	// Use this for initialization
	void Start () {
		server = this.GetComponent<SocketServer> ();
		if(FirstCamera != null)
			sssfc = FirstCamera.GetComponent<ScreenshotScript> ();
		if(ThirdCamera != null)
			ssstc = ThirdCamera.GetComponent<ScreenshotScript> ();
		if (DepthCamera != null)
			sssdc = DepthCamera.GetComponent<ScreenshotScript> ();
		if (SphereCamera != null)
			ssssc = SphereCamera.GetComponent<ScreenshotScript> ();
		
		if (optionPanel == null)
			optionPanel = GameObject.Find ("InitialObject").GetComponent<InitScript> ();
	}

	public string Parse(byte[] command){
		uint cmd = command [0];
		if (cmd == 0) {  //close
			server.Restart ();
			return "Client lost connection";
		} else if (cmd == 1) {  //KeyDown
			string str = System.Text.Encoding.ASCII.GetString (new[]{ command [1] });
			KeyCode k = (KeyCode)System.Enum.Parse (typeof(KeyCode), str.ToUpper ());
			Evt evt = (Evt)System.Enum.Parse (typeof(Evt), "Down");
			InputManager.SetKeyEvt (k, evt);
			return string.Format ("Key {0} Down", k.ToString ());
		} else if (cmd == 2) {  //KeyUp
			string str = System.Text.Encoding.ASCII.GetString (new[]{ command [1] });
			KeyCode k = (KeyCode)System.Enum.Parse (typeof(KeyCode), str.ToUpper ());
			Evt evt = (Evt)System.Enum.Parse (typeof(Evt), "Up");
			InputManager.SetKeyEvt (k, evt);
			return string.Format ("Key {0} Up", k.ToString ());
		} else if (cmd == 3) {  //KeyPress
			string str = System.Text.Encoding.ASCII.GetString (new[]{ command [1] });
			KeyCode k = (KeyCode)System.Enum.Parse (typeof(KeyCode), str.ToUpper ());
			Evt evt = (Evt)System.Enum.Parse (typeof(Evt), "Press");
			InputManager.SetKeyEvt (k, evt);
			return string.Format ("Key {0} Press", k.ToString ());
		} else if (cmd == 4) {  //Speed
			byte[] bytes = server.GetByte (4, true);
			float speed = BitConverter.ToSingle (bytes, 0);
			this.GetComponent<UnityStandardAssets.Characters.FirstPerson.ControllerScript> ().Speed = speed;
			return string.Format ("Set speed to {0} ", speed);
		} else if (cmd == 5) {  //RSpeed
			byte[] bytes = server.GetByte (4, true);
			float speed = BitConverter.ToSingle (bytes, 0);
			this.GetComponent<UnityStandardAssets.Characters.FirstPerson.ControllerScript> ().RotateSpeed = speed;
			return string.Format ("Set RotateSpeed to {0} ", speed);
		} else if (cmd == 6) {  //getPos
			Vector3 pos = this.transform.position;
			byte[] buff = new byte[sizeof(float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes (pos.x), 0, buff, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (pos.y), 0, buff, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (pos.z), 0, buff, 8, 4);
			server.Send (buff);
			return "Get Position";
		} else if (cmd == 7) {  //FPS
			if (FirstCamera == null || sssfc == null)
				return "Get FPS failed";
			byte[] bytes = sssfc.getImage ().EncodeToPNG ();
			server.Send (bytes.Length);
			server.Send (bytes);
			return "Get FPS";
		} else if (cmd == 8) {  //getRot
			Vector3 rot = this.transform.localEulerAngles;
			byte[] buff = new byte[sizeof(float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes (rot.x), 0, buff, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (rot.y), 0, buff, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (rot.z), 0, buff, 8, 4);
			server.Send (buff);
			return "Get Rotation";
		} else if (cmd == 9) {  //setPos
			byte[] bytes = server.GetByte (12, true);
			float x = BitConverter.ToSingle (bytes, 0);
			float y = BitConverter.ToSingle (bytes, 4);
			float z = BitConverter.ToSingle (bytes, 8);
			this.transform.localPosition = new Vector3 (x, y, z);
			return string.Format ("Set Position to ({0}, {1}, {2})", x, y, z);
		} else if (cmd == 0x0a) {  //setRot
			byte[] bytes = server.GetByte (12, true);
			float x = BitConverter.ToSingle (bytes, 0);
			float y = BitConverter.ToSingle (bytes, 4);
			float z = BitConverter.ToSingle (bytes, 8);
			this.transform.localRotation = Quaternion.Euler (new Vector3 (x, y, z));
			return string.Format ("Set Rotation to ({0}, {1}, {2})", x, y, z);
		} else if (cmd == 0x0b) { //getDepth
			if(DepthCamera == null || sssdc == null)
				return "Get Depth failed";
			//sssdc.ScreenShot ();
			byte[] bytes = sssdc.getImage ().EncodeToPNG ();
			server.Send (bytes.Length);
			server.Send (bytes);
			return "Get Depth";
		} else if(cmd == 0x0c){ //setTimeScale
			byte[] bytes = server.GetByte(4, true);
			float scale = BitConverter.ToSingle (bytes, 0);
			optionPanel.TimeScale = scale;
			return string.Format ("Set TimeScale to {0}", scale);
		} else if(cmd == 0x0d){ //getTimeScale
			float scale = optionPanel.TimeScale;
			byte[] buff = new byte[sizeof(float)];
			Buffer.BlockCopy (BitConverter.GetBytes (scale), 0, buff, 0, 4);
			server.Send (buff);
			return "Get TimeScale";
		} else if(cmd == 0x0e){ //setRandPos
			optionPanel.OnRandPos();
			return "Set Random Position";
		} else if(cmd == 0x0f){ //getSpherical
			if (SphereCamera == null || ssssc == null)
				return "Get Spherical failed";
			byte[] bytes = ssssc.getImage ().EncodeToPNG ();
			server.Send (bytes.Length);
			server.Send (bytes);
			return "Get Spherical";
		} else if (cmd == 0xff) {  //String
			uint sz = command[1];
			return server.GetString ((int)sz);
		}

		return "Some thing cannot analyze";
	}
	/*
	public void Parse(string command){
		string[] cmds = command.Split (" ".ToCharArray());
		if (cmds [0] == "FPS") {
			if (FirstCamera == null)
				return;
			byte[] bytes = sssfc.getImage ().EncodeToPNG ();
			server.Send (bytes.Length);
			server.Send (bytes);
		} else if (cmds [0] == "TPS") {
			if (ThirdCamera == null)
				return;
			byte[] bytes = ssstc.getImage ().EncodeToPNG ();
			server.Send (bytes.Length);
			server.Send (bytes);
		} else if (cmds [0] == "Key") {
			KeyCode k = (KeyCode)System.Enum.Parse (typeof(KeyCode), cmds [1].ToUpper ());
			Evt evt = (Evt)System.Enum.Parse (typeof(Evt), cmds [2]);  //Up Down Press
			InputManager.SetKeyEvt (k, evt);
		} else if (cmds [0] == "Pos") {
			Vector3 pos = GameObject.Find ("Controller").transform.position;
			print (pos);
			byte[] buff = new byte[sizeof(float) * 3];
			Buffer.BlockCopy (BitConverter.GetBytes (pos.x), 0, buff, 0, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (pos.y), 0, buff, 4, 4);
			Buffer.BlockCopy (BitConverter.GetBytes (pos.z), 0, buff, 8, 4);
			server.Send (buff);
		}
	}*/
}
