using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCameraScript : MonoBehaviour {

	private Material mat;
	private Camera cam;

	void Awake (){
		mat = new Material (Shader.Find ("Custom/DepthGrayscale"));
		cam = this.GetComponent<Camera> ();
	}

	void Start () {
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination){
		Graphics.Blit(source,destination,mat);
	}
}
