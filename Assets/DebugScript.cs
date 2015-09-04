using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DebugScript : MonoBehaviour {
	
	public DroneConnect drone;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Text> ().text = drone.getRawNavData ().magneto.ToString ();
//		AR.Drone.Data.Navigation.Vector3 velocity = drone.getVelocity ();
//
//		float x = (float) Math.Round(velocity.X * 100, 2);
//		float y = (float) Math.Round(velocity.Y * 100, 2);
//		float z = (float) Math.Round(velocity.Z * 100, 2);
//
//		//GetComponent<Text> ().text = x + ",\n" + y + ",\n" + z;
//
//		if(y < 0){
//			GetComponent<Text> ().text = y + "\nUP";	
//		} else {
//			GetComponent<Text> ().text = y + "\nDOWN";
//		}
	}
}
