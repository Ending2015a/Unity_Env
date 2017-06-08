using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandPosGenerator : MonoBehaviour {

	public string[] floorTags;
	private Bounds []bounds;

	void Awake(){
		//GenerateBounds ();
	}

	// Use this for initialization

	public void GenerateBounds(){
		bounds = new Bounds [floorTags.Length];
		for (int num = 0; num < floorTags.Length; num++) {
			GameObject[] objs = GameObject.FindGameObjectsWithTag (floorTags[num]);
			foreach (GameObject obj in objs) {
				if (obj.GetComponent<MeshFilter> () != null) {
					if (bounds [num].size == Vector3.zero)
						bounds [num] = obj.GetComponent<Renderer> ().bounds;
					else
						bounds[num].Encapsulate(obj.GetComponent<Renderer> ().bounds);
				}
			}
			LogWriter.Log (string.Format ("Create tag \'{0}\' bounds: {1}", floorTags [num], bounds [num].ToString ()));
		}
	}

	public Vector3 getRandPos(){
		int floor = Random.Range (0, floorTags.Length);
		return getRandPos (floor);
	}

	public Vector3 getRandPos(string tags){
		for (int num = 0; num < floorTags.Length; num++) {
			if (floorTags [num] == tags) {
				return getRandPos (num);
			}
		}
		LogWriter.Error (string.Format ("Cannot find the tag {0}", tags));
		return getRandPos (0);
	}

	public Vector3 getRandPos(int tags){
		GameObject[] objs = GameObject.FindGameObjectsWithTag (floorTags [tags]);

		Bounds bnd = objs[Random.Range(0, objs.Length)].GetComponent<Renderer>().bounds;
		Vector3 min = bnd.min;
		Vector3 max = bnd.max;
		Vector3 pos = new Vector3 (Random.Range (min.x, max.x), (min.y + max.y) / 2 + this.transform.localScale.y, Random.Range (min.z, max.z));
		return pos;
	}

	public string[] getTagList(){
		return floorTags;
	}

}
