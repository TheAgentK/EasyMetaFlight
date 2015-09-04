using UnityEngine;
using System.Collections;

public class gMapsDrone : MonoBehaviour {
	
	public DroneConnect drone;
	
	public Transform bigScreenGMap;
	public droneCamFrontController camControll;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {

		}
	}

	public bool isActive(){
		return bigScreenGMap.gameObject.activeSelf;
	}
	
	public void toggleCam(){
		if (!isActive()) {
			if(camControll.isActive()){
				camControll.SetActive(false);
			}		
		}
		bigScreenGMap.gameObject.SetActive (!isActive());
	}

	public void SetActive(bool state){
		bigScreenGMap.gameObject.SetActive (state);
	}
}
