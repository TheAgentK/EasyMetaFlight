using UnityEngine;
using System.Collections;
using Meta;

public class droneCamFrontController : MonoBehaviour {

	public DroneConnect drone;

	public gMapsDrone gmapsControll;
	public Transform bigScreenCam;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			GetComponent<Renderer> ().material.mainTexture = drone.getVideoStreamTexture ();
		}
	}

	public bool isActive(){
		return bigScreenCam.gameObject.activeSelf;
	}

	public void toggleCam(){
		if (!isActive ()) {
			if (gmapsControll.isActive ()) {
				gmapsControll.SetActive (false);
			}
		}
		if (isActive()) {
			bigScreenCam.GetComponent<Renderer>().material.mainTexture = drone.getVideoStreamTexture ();
		} else {
			Texture2D blackTexture = new Texture2D (1, 1);
			blackTexture.SetPixel (0, 0, Color.black);
			blackTexture.Apply ();
			bigScreenCam.GetComponent<Renderer>().material.mainTexture = blackTexture;
		}
		bigScreenCam.gameObject.SetActive (!isActive());
	}

	public void SetActive(bool state){
		bigScreenCam.gameObject.SetActive (state);
	}
}
