using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager{

	private static VirtualInputHandler handler;

	public static VirtualInputHandler InputHandler{
		get{
			if (handler == null) {
				handler = GameObject.FindObjectOfType (typeof(VirtualInputHandler)) as VirtualInputHandler;
			}
			if (handler == null)
				Debug.LogError ("Cant find VirtualInputHandler");
			return handler;
		}

	}

	public static bool GetKey(KeyCode k){
		return InputHandler.GetKey (k);
	}

	public static bool GetKeyDown(KeyCode k){
		return InputHandler.GetKeyDown (k);
	}

	public static bool GetKeyUp(KeyCode k){
		return InputHandler.GetKeyUp (k);
	}

	public static void SetKeyDown(KeyCode k){
		InputHandler.SetKeyEvt (k, Evt.Down);
	}

	public static void SetKeyUp(KeyCode k){
		InputHandler.SetKeyEvt (k, Evt.Up);
	}

	public static void SetKeyPress(KeyCode k){
		InputHandler.SetKeyEvt (k, Evt.Press);
	}

	public static void SetKeyEvt(KeyCode k, Evt evt){
		InputHandler.SetKeyEvt (k, evt);
	}
}
