using UnityEngine;
using System.Collections;
using Meta;
using UnityEngine.UI;

public class dronePitch : MonoBehaviour {
	public Transform numberLine;
	public Transform smallLine;
	public Transform nullLine;

	public Transform rollIndicator;

	public int deltaDegree = 15;
	public int deltaDegreeTotal = 45;

	public Renderer bigScreenCam;

	public DroneConnect drone;
	private float distance;
	// Use this for initialization
	void Start () {
		for (int degree = deltaDegreeTotal; degree >= -deltaDegreeTotal; degree -= 5) {
			createVerticalLine(degree, degree);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (drone.isDroneConnected) {
			float yDistance = -drone.getPitchAsDegree () / 10;
			updatePitchGUI (yDistance);
		} else {
			updatePitchGUI (0f);
		}
	}

	void updatePitchGUI(float actualDistance){		
		for(int childIndex = 0; childIndex < transform.childCount; childIndex ++)
		{
			Transform child = this.transform.GetChild(childIndex);
			float currentPos = child.localPosition.y;
			float newDistance = child.localPosition.y-distance+actualDistance;

			child.localPosition = new Vector3(0f, newDistance, 0f);
			if(child.name == "0 - indicator"){
				//child.localRotation = Quaternion.Euler (0, 0, (drone.getRollAsDegree () * -1));
			}
			if(bigScreenCam.enabled) {
				Helper.colorSprites(child, Color.green);
			} else {
				Helper.colorSprites(child, Color.white);
			}

			if(currentPos <= (deltaDegree/10f) && currentPos >= -(deltaDegree/10f)){
				child.gameObject.SetActive(true);
			} else {
				child.gameObject.SetActive(false);
			}
		}
		if(bigScreenCam.enabled) {
			Helper.colorSprites(rollIndicator, Color.green);
		} else {
			Helper.colorSprites(rollIndicator, Color.white);
		}
		distance = actualDistance;
	}

	void createVerticalLine(float position, int textDegree){
		if(position % 10 == 0)
		{
			Transform newObject = (Transform)Instantiate(numberLine, new Vector3(0f,0f,0f), this.transform.rotation);
			newObject.parent = this.transform;
			newObject.localScale = new Vector3(1f,1f,1f);
			newObject.localPosition = new Vector3(0f,position/10f,0f);	
			newObject.name = textDegree+"";
			if(bigScreenCam.enabled) {
				Helper.colorSprites(newObject, Color.green);
			}

			if(position == 0){
				Transform newObject2 = (Transform)Instantiate(nullLine, new Vector3(0f,0f,0f), this.transform.rotation);
				newObject2.parent = this.transform;
				newObject2.localScale = new Vector3(30f,0.3f,1f);
				newObject2.localPosition = new Vector3(0f,position/10f,0f);
				newObject2.name = textDegree + " - indicator";
				if(bigScreenCam.enabled) {
					Helper.colorSprites(newObject2, Color.green);
				}
			}
			
			// Text aktualisieren
			Transform canvasObject = newObject.FindChild("MGUI.Canvas 1");
			if(canvasObject == null)
				return;
			
			Transform textObjectLeft = canvasObject.FindChild("MGUI.Text-left");
			if(textObjectLeft == null)
				return;
			Text textFieldLeft = textObjectLeft.GetComponent<Text>();
			textFieldLeft.color = Color.green;
			textFieldLeft.text = textDegree.ToString("D2");

			Transform textObjectRight = canvasObject.FindChild("MGUI.Text-right");
			if(textObjectRight == null)
				return;
			Text textFieldRight = textObjectRight.GetComponent<Text>();
			textFieldRight.color = Color.green;
			textFieldRight.text = textDegree.ToString("D2");
		} else {
			Transform newObject = (Transform)Instantiate(smallLine, new Vector3(0f,0f,0f), this.transform.rotation);
			newObject.parent = this.transform;
			newObject.localScale = new Vector3(1f,1f,1f);
			newObject.localPosition = new Vector3(0f,position/10f,0f);
			newObject.name = textDegree+"";
		}
	}
}