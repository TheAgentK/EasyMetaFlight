using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class Wlan : MonoBehaviour {
	public DroneConnect drone;
	public Text message;
	public Text wlanStrength;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			if (drone.getWlanStrenth() < 99f) {
				message.text = "W-LAN-Signal schwach!";
				message.color = Color.red;
			}
			wlanStrength.text = drone.determineWifiStrength().ToString() + " %";
		}
	}
}
