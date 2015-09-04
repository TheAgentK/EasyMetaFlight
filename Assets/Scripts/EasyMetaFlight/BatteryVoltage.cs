using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class BatteryVoltage : MonoBehaviour {

	public DroneConnect drone;

	// Use this for initialization
	void Start () {
		if (drone.isDroneConnected)
			GetComponent<Text> ().text = drone.getBatteryVoltage ().ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected)
			GetComponent<Text> ().text = drone.getBatteryVoltage ().ToString();
	}	
}
