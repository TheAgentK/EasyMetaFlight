using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class PitchDegree : MonoBehaviour {
	
	public DroneConnect drone;
	
	// Use this for initialization
	void Start () {
		if (drone.isDroneConnected)
			GetComponent<Text> ().text = drone.getPitchAsDegree ().ToString("00");
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
//			Debug.Log (drone.getPitchAsDegree ().ToString ());
			GetComponent<Text> ().text = drone.getPitchAsDegree ().ToString ("00");
		}
	}	
}
