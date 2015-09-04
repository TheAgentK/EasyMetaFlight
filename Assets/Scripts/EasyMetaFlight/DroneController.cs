using UnityEngine;
using System.Collections;
using XInputDotNetPure;

/// <summary>
/// Hanlder to Controlle the Drone with the xboxe 360 Controller
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class DroneController : MonoBehaviour {

	/// <summary>
	/// Log the unhandled exception.
	/// </summary>
	public DroneConnect drone;
	
	// Gamepad variables
	private bool playerIndexSet = false; 
	private XInputDotNetPure.PlayerIndex playerIndex;
	private GamePadState state;
	private GamePadState prevState;

	// Indicates that the startButton is Pressed
	private bool startButtonPressed = false;
	private bool rightShoulderPressed = false;
	private bool leftShoulderPressed = false;
	private bool camTogglePressed = false;
	
	/// <summary>
	/// Indicates that the Hover Comand is send
	/// </summary>
	private bool isHoverSend = false;

	public droneCamFrontController camControll;
	public gMapsDrone gmapsControll;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		
		updateGamepadState ();

		if(drone.isDroneConnected) {
			MoveDrone();
		}

		// toggle DroneCam Image - big-Small
		if (state.Buttons.RightShoulder.Equals (ButtonState.Pressed) && !rightShoulderPressed) {
			camControll.toggleCam();
			rightShoulderPressed = true;
		}
		if (!state.Buttons.RightShoulder.Equals (ButtonState.Pressed))
			rightShoulderPressed = false;

		// toggle DroneCam Image - big-Small
		if (state.Buttons.LeftShoulder.Equals (ButtonState.Pressed) && !leftShoulderPressed) {
			gmapsControll.toggleCam();
			leftShoulderPressed = true;
		}
		if (!state.Buttons.LeftShoulder.Equals (ButtonState.Pressed))
			leftShoulderPressed = false;

		// ToggleCamera
		if (state.Buttons.B.Equals (ButtonState.Pressed) && camControll.isActive() && !camTogglePressed) {
			drone.toggleDroneCame();
			camTogglePressed = true;
		}
		if (!state.Buttons.B.Equals (ButtonState.Pressed))
		    camTogglePressed = false;

		// exit application
		if (Input.GetKey ("escape") || state.Buttons.Back.Equals (ButtonState.Pressed)) {
			Application.Quit ();
			Debug.LogWarning ("Exit Application");
		}
	}

	void MoveDrone(){
		// Start or land the drone
		if (state.Buttons.Start.Equals(ButtonState.Pressed) && !startButtonPressed) {
			if (drone.isLanded) {
				drone.getDroneClient().ResetEmergency();
				drone.getDroneClient().FlatTrim();
				drone.getDroneClient().Takeoff();
				drone.isLanded = false;
			} else {
				drone.getDroneClient().Land();
				drone.isLanded = true;
			}
			startButtonPressed = true;
		}
		
		if (!state.Buttons.Start.Equals(ButtonState.Pressed))
			startButtonPressed = false;

		if(!drone.isLanded){
			// Move the drone
			var pitch = -state.ThumbSticks.Left.Y;
			var roll = state.ThumbSticks.Left.X;
			var gaz = state.Triggers.Right - state.Triggers.Left;
			var yaw = state.ThumbSticks.Right.X;

			//Debug.Log (pitch + ", " + roll + ", " + gaz + ", " + yaw);

			if(pitch != 0 || roll != 0 || gaz != 0 || yaw != 0){
				drone.getDroneClient ().Progress (AR.Drone.Client.Command.FlightMode.Progressive, pitch: pitch, roll: roll, gaz: gaz, yaw: yaw);
				isHoverSend = false;
			} else {
				if(!isHoverSend) {
					Debug.Log ("Hovercomand send");
					drone.getDroneClient().Hover();
					isHoverSend = true;
				}
			}
		}
	}

	/// <summary>
	/// Updates the state of the gamepad.
	/// </summary>
	void updateGamepadState(){
		// Find a PlayerIndex, for a single player game
		// Will find the first controller that is connected and use it
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex = testPlayerIndex;
					playerIndexSet = true;
				}
			}
		}
		
		prevState = state;
		state = GamePad.GetState(playerIndex);
		drone.isControllerConnected = state.IsConnected;
	}
}
