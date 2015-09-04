using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BatteryLow : MonoBehaviour {
	
	public DroneConnect drone;
	public Text message;

	public SpriteRenderer batterIndicator;

	public Sprite batter100;
	public Sprite batter75;
	public Sprite batter50;
	public Sprite batter25;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			if (drone.isBatteryLow ()) {
				message.text = "Batterie schwach!";
				message.color = Color.red;
			}

			float percent = drone.getBatteryPercentage();
			if(percent <= 100 && percent > 75) {
				batterIndicator.sprite = batter100;
			} else if(percent <= 75 && percent > 50){
				batterIndicator.sprite = batter75;
			} else if(percent <= 50 && percent > 25){
				batterIndicator.sprite = batter50;
			} else {
				batterIndicator.sprite = batter25;
			}
		}
	}
}
