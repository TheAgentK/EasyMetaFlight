using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RollDegree : MonoBehaviour {
	
	public DroneConnect drone;
	
	// Use this for initialization
	void Start () {
		if (drone.isDroneConnected)
			GetComponent<Text> ().text = drone.getRollAsDegree ().ToString("00");
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected)
			GetComponent<Text> ().text = drone.getRollAsDegree ().ToString("00");
	}	
}
