using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class KeyEvt{
	public KeyCode keycode;
	public int Life;

	public KeyEvt(KeyCode k, int life){
		keycode = k;
		Life = life;
	}
	public bool Sub(){
		Life -= 1;
		if (Life == 0)
			return false;
		return true;
	}
}

public enum Evt{ Up=0, Down=1, Press=2};

public class VirtualInputHandler : MonoBehaviour {

	private bool []Key;
	private List<KeyEvt> downEvt = new List<KeyEvt>();
	private List<KeyEvt> upEvt = new List<KeyEvt>();

	void Start(){
		Key = new bool[System.Enum.GetValues (typeof(KeyCode)).Length];
	}

	void Update(){
		for (int i = 0; i < downEvt.Count; i++) {
			if (!downEvt [i].Sub ()) {
				downEvt.Remove (downEvt [i]);
			}
		}

		for (int i = 0; i < upEvt.Count; i++) {
			if (!upEvt [i].Sub ()) {
				upEvt.Remove (upEvt [i]);
			}
		}
	}

	public void SetKeyEvt(KeyCode k, Evt evt){
		if (evt == Evt.Up) {  //up
			int i = downEvt.FindIndex (n => n.keycode == k);
			if(i != -1)downEvt.RemoveAt (i);
			upEvt.Add (new KeyEvt (k, 1));
			Key [(int)k] = false;
		} else if (evt == Evt.Down) {  //down
			int i = upEvt.FindIndex(n => n.keycode == k);
			if(i != -1)upEvt.RemoveAt(i);
			downEvt.Add (new KeyEvt (k, 1));
			Key [(int)k] = true;
		} else if (evt == Evt.Press) {  //press
			downEvt.Add(new KeyEvt(k, 1));
		}
	}

	public bool GetKey(KeyCode k){
		return Key[(int)k];
	}

	public bool GetKeyDown(KeyCode k){
		bool st = downEvt.Exists (n => n.keycode == k);
		return st;
	}

	public bool GetKeyUp(KeyCode k){
		bool st = upEvt.Exists (n => n.keycode == k);
		return st;
	}
}
