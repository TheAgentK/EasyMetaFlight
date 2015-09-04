using UnityEngine;
using System.Collections;

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
