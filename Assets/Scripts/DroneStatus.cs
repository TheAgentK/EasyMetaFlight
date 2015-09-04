﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class DroneStatus : MonoBehaviour {
	
	public DroneConnect drone;
	public DisplayMessage message;
	
	private bool connectMessageShow = false;
	private bool disconnectMessageShow = false;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			GetComponent<Renderer>().material.color = Color.green;
			if(!connectMessageShow) {
				message.showMessage("Drohne verbunden!");
				connectMessageShow = true;
				disconnectMessageShow = false;
			}
		} else {
			GetComponent<Renderer>().material.color = Color.red;
			if(!disconnectMessageShow) {
				message.showMessage("Drohne disconnected!", Color.red);
				disconnectMessageShow = true;
				connectMessageShow = false;
			}
		}
	}
}
