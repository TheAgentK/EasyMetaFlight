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

public class DroneConnect : MonoBehaviour {

	private DroneClient droneClient;
	private NavigationData navigationData;
	private NavigationPacket navigationPacket;
	private VideoPacketDecoderWorker videoPacketDecoderWorker;
	// byte array which will be filled by the camera data
	private byte[] data;
	// Texture used for the camera content
	private Texture2D cameraTexture;

	private AR.Drone.Client.Configuration.VideoChannelType actualVideoChannel = AR.Drone.Client.Configuration.VideoChannelType.Horizontal;

	private const float RadianToDegree = (float) (180.0f/Math.PI);

	public bool isDroneConnected = false;
	public bool isControllerConnected = false;
	public bool isLanded = false;

	private float prevAltiUp = 0;
	private float currentAltiUp = 0;
	private float prevAltiDown = 0;
	private float currentAltiDown = 0;
	private MaxBuffer<float> altitudeBuffer = new MaxBuffer<float> (5);

	private float prevBattery = 0;
	private float currentBattery = 0;

	// wlanclient for signal strength
	private WlanClient wlanClient;

	// Width and height if the camera
	private int width = 640;
	private int height = 360;

	// Use this for initialization
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

	// Update is called once per frame
	void Update () {;
		if (isDroneConnected) {
		}
	}

	// Called if the gameobject is destroyed
	void OnDestroy(){
		droneClient.Land();
		droneClient.Stop();
		droneClient.Dispose ();
		videoPacketDecoderWorker.Stop ();
		videoPacketDecoderWorker.Dispose();
	}

	public NavdataBag getRawNavData(){
		return navigationData.raw;
	}

	public DroneClient getDroneClient(){
		return droneClient;
	}

	public float getPitch(){
		//Debug.Log ("Pitch: " + navigationData.Pitch);
		return navigationData.Pitch;
	}
	public float getPitchAsDegree(){
		float pitch = getPitch ();
		pitch = pitch * RadianToDegree;
		pitch = (float) Math.Round(pitch, 0);
		return pitch;
	}

	public float getRoll(){
		//Debug.Log ("Roll: " + navigationData.Roll);
		return navigationData.Roll;
	}
	public float getRollAsDegree(){
		float roll = getRoll ();
		roll = roll * RadianToDegree;
		roll = (float) Math.Round(roll, 0);
		return roll;
	}

	public float getYaw(){
		//Debug.Log ("Yaw: " + navigationData.Yaw);
		return navigationData.Yaw;
	}
	public float getYawAsDegree(){
		float yaw = getYaw ();
		yaw = yaw * RadianToDegree;
		yaw = (float) Math.Round(yaw, 0);
		return yaw;
	}

	public float getAltitude(){
		//Debug.Log ("Yaw: " + navigationData.Yaw);
		return (float) Math.Round(navigationData.Altitude, 2);
	}

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

	public Magneto getMagneto(){
		Debug.Log (navigationData.Magneto.heading_angle);
		return navigationData.Magneto;
	}
	
	public bool isBatteryLow(){
		//Debug.Log ("Yaw: " + navigationData.Yaw);
		return navigationData.Battery.Low;
	}
	public float getBatteryPercentage(){
		return navigationData.Battery.Percentage;
	}
	public float getBatteryVoltage(){
		prevBattery = currentBattery;
		currentBattery = (float) Math.Round(navigationData.Battery.Voltage, 1);
		
		float diffBattery = (prevBattery + currentBattery) / 2;
		return (float) Math.Round(diffBattery, 1);
	}

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
