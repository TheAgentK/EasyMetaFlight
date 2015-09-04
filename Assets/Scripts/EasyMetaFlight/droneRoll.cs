using UnityEngine;
using System.Collections;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class droneRoll : MonoBehaviour {

	public DroneConnect drone;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			transform.localRotation = Quaternion.Euler (0, 0, -drone.getRollAsDegree ());
		}
	}
}
