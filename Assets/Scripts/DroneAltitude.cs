using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class DroneAltitude : MonoBehaviour {

	public DroneConnect drone;
	public Text altitudeText;

	public SpriteRenderer arrowUp;
	public SpriteRenderer arrowDown;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			float orgNumber = drone.getAltitude();
			//float orgNumber = 1.5f;
			altitudeText.text = Helper.FloatToDezimalWithlength(orgNumber, 5);
			if (drone.isMovingUp ()) {
				arrowUp.enabled = true;
				arrowDown.enabled = false;
			} else if (drone.isMovingDown ()) {
				arrowUp.enabled = false;
				arrowDown.enabled = true;
			} else {
				arrowUp.enabled = false;
				arrowDown.enabled = false;
			}
		} else {
			arrowUp.enabled = false;
			arrowDown.enabled = false;
		}
	}
}
