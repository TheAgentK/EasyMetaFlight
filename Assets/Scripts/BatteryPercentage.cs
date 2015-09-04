using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BatteryPercentage : MonoBehaviour {

	public DroneConnect drone;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {

			GetComponent<Text> ().text = drone.getBatteryPercentage ().ToString () + " %";
			
		}
	}
}
