 using UnityEngine;
using System;
using System.Collections;
using AR.Drone.Client;
using AR.Drone.Client.Configuration;
using AR.Drone.Media;
using AR.Drone.Video;
using AR.Drone.Data;
using AR.Drone.Data.Navigation;
using FFmpeg.AutoGen;
using XInputDotNetPure;
using NativeWifi;
using AR.Drone.Data.Navigation.Native;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class DroneConnect : MonoBehaviour {
	
	/// <summary>
	/// Handle the connection Client to the Drone
	/// @warning Value is set automatically
	/// </summary>
	private DroneClient droneClient;
	/// <summary>
	/// the NavData from the droneClient
	/// @warning Value is set automatically
	/// </summary>
	private NavigationData navigationData;
	/// <summary>
	/// navPacket from the droneClient
	/// @warning Value is set automatically
	/// </summary>
	private NavigationPacket navigationPacket;
	/// <summary>
	/// The Video Decoder Class
	/// @warning Value is set automatically
	/// </summary>
	private VideoPacketDecoderWorker videoPacketDecoderWorker;
	/// <summary>
	/// byte array which will be filled by the camera data
	/// @warning Value is set automatically
	/// </summary>
	private byte[] data;
	/// <summary>
	/// Texture used for the camera content
	/// @warning Value is set automatically
	/// </summary>
	private Texture2D cameraTexture;
	
	/// <summary>
	/// Saves with Videocamera is selected
	/// @warning Value is set automatically
	/// </summary>
	private AR.Drone.Client.Configuration.VideoChannelType actualVideoChannel = AR.Drone.Client.Configuration.VideoChannelType.Horizontal;
	
	/// <summary>
	/// constant for calculating Radiant to Degree
	/// </summary>
	private const float RadianToDegree = (float) (180.0f/Math.PI);
	
	/// <summary>
	/// show wether or not the drone is connected
	/// @warning Value is set automatically
	/// </summary>
	public bool isDroneConnected = false;
	/// <summary>
	/// show wether or not the controller is connected
	/// @warning Value is set automatically
	/// </summary>
	public bool isControllerConnected = false;
	/// <summary>
	/// show wether or not the drone is landed
	/// @warning Value is set automatically
	/// </summary>
	public bool isLanded = false;

	/// <summary>
	/// Buffer to hold entrys of the altitude to check if the drone is rising or not
	/// @warning Value is set automatically
	/// </summary>
	private MaxBuffer<float> altitudeBuffer = new MaxBuffer<float> (5);
	
	/// <summary>
	/// the previous batter percentage
	/// @warning Value is set automatically
	/// </summary>
	private float prevBattery = 0;
	/// <summary>
	/// the current battery percentage
	/// @warning Value is set automatically
	/// </summary>
	private float currentBattery = 0;
	
	/// <summary>
	/// wlanclient for signal strength
	/// @warning Value is set automatically
	/// </summary>
	private WlanClient wlanClient;
	
	/// <summary>
	/// width of the camera
	/// @warning static value
	/// </summary>
	private int width = 640;
	/// <summary>
	/// height of the camera
	/// @warning static value
	/// </summary>
	private int height = 360;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () { 
		Debug.Log("Start DroneObserver");
		// initialize data array
		data = new byte[width*height*3];
		cameraTexture = new Texture2D (width, height);
		// Initialize drone
		videoPacketDecoderWorker = new VideoPacketDecoderWorker(PixelFormat.RGB24, true, OnVideoPacketDecoded);
		videoPacketDecoderWorker.Start();

		droneClient = new DroneClient ("192.168.1.1");
		//droneClient = new DroneClient ("127.0.0.1");
		droneClient.UnhandledException += HandleUnhandledException;
		droneClient.VideoPacketAcquired += OnVideoPacketAcquired;
		droneClient.NavigationDataAcquired += navData => navigationData = navData;

		droneClient.FlatTrim ();

		videoPacketDecoderWorker.UnhandledException += HandleUnhandledException;
		droneClient.Start ();

		Settings settings = new Settings ();
		settings.Video.Codec = VideoCodecType.H264_720P;
		droneClient.Send (settings);
		droneClient.AckControlAndWaitForConfirmation ();

		switchDroneCamera (AR.Drone.Client.Configuration.VideoChannelType.Horizontal);

		isDroneConnected = droneClient.IsConnected;

		if(!isDroneConnected){
			Debug.LogError("Drone not Connected. Retry!!!");	
		}
		if(isDroneConnected){
			Debug.LogWarning("Drone Connected!!!");	
		}

		// determine connection
		wlanClient = new WlanClient();
	}
	
	/// <summary>
	/// Called if the gameobject is destroyed
	/// </summary>
	void OnDestroy(){
		droneClient.Land();
		droneClient.Stop();
		droneClient.Dispose ();
		videoPacketDecoderWorker.Stop ();
		videoPacketDecoderWorker.Dispose();
	}
	
	/// <summary>
	/// get the raw NavData from AR.Drone
	/// </summary>
	public NavdataBag getRawNavData(){
		return navigationData.raw;
	}
	
	/// <summary>
	/// get the DroneClient
	/// </summary>
	public DroneClient getDroneClient(){
		return droneClient;
	}
	
	/// <summary>
	/// get the pitch as radian
	/// </summary>
	public float getPitch(){
		return navigationData.Pitch;
	}
	/// <summary>
	/// get ppitch as degree
	/// </summary>
	public float getPitchAsDegree(){
		float pitch = getPitch ();
		pitch = pitch * RadianToDegree;
		pitch = (float) Math.Round(pitch, 0);
		return pitch;
	}
	
	/// <summary>
	/// get roll as radian
	/// </summary>
	public float getRoll(){
		//Debug.Log ("Roll: " + navigationData.Roll);
		return navigationData.Roll;
	}
	/// <summary>
	/// get roll as degree
	/// </summary>
	public float getRollAsDegree(){
		float roll = getRoll ();
		roll = roll * RadianToDegree;
		roll = (float) Math.Round(roll, 0);
		return roll;
	}
	
	/// <summary>
	/// get yaw as radian
	/// </summary>
	public float getYaw(){
		//Debug.Log ("Yaw: " + navigationData.Yaw);
		return navigationData.Yaw;
	}
	/// <summary>
	/// get yaw as degree
	/// </summary>
	public float getYawAsDegree(){
		float yaw = getYaw ();
		yaw = yaw * RadianToDegree;
		yaw = (float) Math.Round(yaw, 0);
		return yaw;
	}
	
	/// <summary>
	/// get altitude in meter
	/// </summary>
	public float getAltitude(){
		return (float) Math.Round(navigationData.Altitude, 2);
	}
	
	/// <summary>
	/// true if the drone is rising
	/// </summary>
	public bool isMovingUp(){
		altitudeBuffer.Add (getAltitude());
		float first = (float)altitudeBuffer.ToArray ().GetValue (0);
		float last = (float)altitudeBuffer.ToArray ().GetValue (altitudeBuffer.ToArray ().GetUpperBound (0));

		if (last > (first + 0.02f)) {
			return true;
		} else {
			return false;
		}
	}
	
	/// <summary>
	/// true if the drone fall
	/// </summary>
	public bool isMovingDown(){
		altitudeBuffer.Add (getAltitude());
		float first = (float)altitudeBuffer.ToArray ().GetValue (0);
		float last = (float)altitudeBuffer.ToArray ().GetValue (altitudeBuffer.ToArray ().GetUpperBound (0));
		
		if (last < (first - 0.02f)) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// get all Parameters from the Magneto
	/// @see Magneto
	/// </summary>
	public Magneto getMagneto(){
		return navigationData.Magneto;
	}
	/// <summary>
	/// check if the batter percentage is under 25%
	/// </summary>
	public bool isBatteryLow(){
		return navigationData.Battery.Low;
	}
	/// <summary>
	/// get battery percentage
	/// </summary>
	public float getBatteryPercentage(){
		return navigationData.Battery.Percentage;
	}
	/// <summary>
	/// ge the current batter voltage in Volts
	/// </summary>
	public float getBatteryVoltage(){
		prevBattery = currentBattery;
		currentBattery = (float) Math.Round(navigationData.Battery.Voltage, 1);
		
		float diffBattery = (prevBattery + currentBattery) / 2;
		return (float) Math.Round(diffBattery, 1);
	}

	/// <summary>
	/// get the W-Lan stength as a value from 1 to 10
	/// </summary>
	public float getWlanStrenth(){
		return navigationData.Wifi.LinkQuality;
	}

	/// <summary>
	/// Determine the wifi strength.
	/// </summary>
	public int determineWifiStrength(){
		int signalQuality = 0;
		foreach (WlanClient.WlanInterface wlanIface in wlanClient.Interfaces)
		{
			try {
				signalQuality = (int)wlanIface.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality;
			}
			catch (System.Exception e ){
				Debug.Log ("No Wifi Connection \n" + e);
			}
		}

		return signalQuality;
	}

	/// <summary>
	/// Switchs the drone camera.
	/// </summary>
	/// <param name="Type">Video channel type.</param>
	private void switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType Type){
		var configuration = new AR.Drone.Client.Configuration.Settings();
		configuration.Video.Channel = Type;
		droneClient.Send(configuration);
	}

	/// <summary>
	/// Toggle the drone cameras from front to bottom and vice versa
	/// </summary>
	public void toggleDroneCame(){
		if(actualVideoChannel.Equals(AR.Drone.Client.Configuration.VideoChannelType.Vertical)){
			switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.Horizontal);
			actualVideoChannel = AR.Drone.Client.Configuration.VideoChannelType.Horizontal;
		}  else {
			switchDroneCamera(AR.Drone.Client.Configuration.VideoChannelType.Vertical);
			actualVideoChannel = AR.Drone.Client.Configuration.VideoChannelType.Vertical;
		}
	}

	/// <summary>
	/// Converts the camera data to a color array and applies it to the texture.
	/// </summary>
	public Texture2D getVideoStreamTexture(){
		int r = 0;
		int g = 0;
		int b = 0;
		int total = 0;
		var colorArray = new Color32[data.Length/3];
		for(var i = 0; i < data.Length; i+=3)
		{
			colorArray[i/3] = new Color32(data[i + 2], data[i + 1], data[i + 0], 1);
			r += data[i + 2];
			g += data[i + 1];
			b += data[i + 0];
			total++;
		}
		r /= total;
		g /= total;
		b /= total;

		cameraTexture.SetPixels32(colorArray);
		cameraTexture.Apply();

		return cameraTexture;
		//return MirrorTexture(cameraTexture, false, true);
	}

	/// <summary>
	/// Log the unhandled exception.
	/// </summary>
	/// <param name="arg1">Arg1.</param>
	/// <param name="arg2">Arg2.</param>
	void HandleUnhandledException (object arg1, System.Exception arg2)
	{
		Debug.Log (arg2); 
	}
	/// <summary>
	/// Determines what happens if a video packet is acquired.
	/// </summary>
	/// <param name="packet">Packet.</param>
	private void OnVideoPacketAcquired(VideoPacket packet)
	{
		if (videoPacketDecoderWorker.IsAlive)
			videoPacketDecoderWorker.EnqueuePacket(packet);
	}
	/// <summary>
	/// Determines what happens if a video packet is decoded.
	/// </summary>
	/// <param name="frame">Frame.</param>
	private void OnVideoPacketDecoded(VideoFrame frame)
	{
		data = frame.Data;
	}
}
