using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCameraScript : MonoBehaviour {

	public Material mat;
	public Camera camera;

	void Awake (){
		camera = this.GetComponent<Camera> ();
	}

	void Start () {
		camera.depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination){
		Graphics.Blit(source,destination,mat);
		//mat is the material which contains the shader
		//we are passing the destination RenderTexture to
	}
}
