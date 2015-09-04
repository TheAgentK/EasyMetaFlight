using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
