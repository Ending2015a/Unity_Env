using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenshotScript : MonoBehaviour {


	private Camera cam;
	public int Width;
	public int Height;
	public bool useDefaultSize;


	private Texture2D newTex;
	private bool shotting = false;
	private RenderTexture target;

	// Use this for initialization
	void Awake () {
		cam = this.GetComponent<Camera> ();
		if (cam == null)
			Debug.LogError ("Cant find Camera in: " + this.name);

		if (useDefaultSize) {
			Width = Screen.width;
			Height = Screen.height;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (InputManager.GetKeyDown (KeyCode.Z)) {
			ScreenShot ();
		}
	}

	private static string ScreenShotName(int Width, int Height){
		Directory.CreateDirectory (string.Format ("{0}/screenshots", Application.dataPath));
		return string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png",
			Application.dataPath, Width, Height, System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
	}

	public void renderToScreen(bool b){
		if (b)
			this.GetComponent<Camera> ().enabled = true;
		else
			this.GetComponent<Camera> ().enabled = false;
	}

	public Texture2D getImage(){
		
		if (!shotting) {
			shotting = true;
			RenderTexture sc = new RenderTexture (Width, Height, 24);
			cam.targetTexture = sc;
			Texture2D screenShot = new Texture2D (Width, Height, TextureFormat.RGB24, false);
			cam.Render ();
			RenderTexture.active = sc;
			screenShot.ReadPixels (new Rect (0, 0, Width, Height), 0, 0);
			cam.targetTexture = target;
			RenderTexture.active = target;
			Destroy (sc);
			newTex = screenShot;

			shotting = false;
		}
		return newTex;
	}

	public void ScreenShot(){
		Texture2D sc = getImage ();

		byte[] bytes = sc.EncodeToPNG ();
		string filename = ScreenShotName (Width, Height);
		System.IO.File.WriteAllBytes (filename, bytes);
		Debug.Log (string.Format ("Took screenshot to: {0}", filename));
	}
}
