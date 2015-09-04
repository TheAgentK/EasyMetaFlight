using UnityEngine;
using System.Collections;
using Meta;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class compass : MonoBehaviour {

	public DroneConnect drone;

	public Transform texture;

	public RectTransform north;
	public RectTransform east;
	public RectTransform south;
	public RectTransform west;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(drone.isDroneConnected){
			Helper.colorSprites(texture, Color.green);

			float camAngle = drone.getRawNavData ().magneto.heading_fusion_unwrapped;
			texture.localRotation = Quaternion.Euler (0, 0, camAngle);

			float negCamAngle = camAngle * -1;
			north.localRotation = Quaternion.Euler (0, 0, negCamAngle);
			east.localRotation = Quaternion.Euler (0, 0, negCamAngle);
			south.localRotation = Quaternion.Euler (0, 0, negCamAngle);
			west.localRotation = Quaternion.Euler (0, 0, negCamAngle);
		}
	}
}
