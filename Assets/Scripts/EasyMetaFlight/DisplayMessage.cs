using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
public class DisplayMessage : MonoBehaviour {
	List<MessageItem> messages = new List<MessageItem>();

	public void showMessage(string message)
	{
		MessageItem item = new MessageItem ();
		item.message = message;

		messages.Add(item);
		UpdateDisplay();
		Invoke("DeleteOldestMessage", 10F);
	}
	public void showMessage(string message, Color color)
	{
		MessageItem item = new MessageItem ();
		item.message = message;
		item.color = color;

		messages.Add(item);
		UpdateDisplay();
		Invoke("DeleteOldestMessage", 10F);
	}

	void DeleteOldestMessage()
	{
		// The following "if statement" protects this function from
		// getting called by SendMessage from another script and
		// crashing.
		if (messages.Count > 0)
		{
			messages.RemoveAt(0);
			UpdateDisplay();
		}
	}

	void UpdateDisplay()
	{
		string formattedMessages = "";

		foreach (MessageItem message in messages)
		{
			if (message.color != default(Color)) {
				string hexColor = Helper.ColorToHex (message.color);
				formattedMessages += "<color=#" + hexColor + ">" + message.message + "</color>" + "\n";
			} else {
				formattedMessages += message.message + "\n";
			}
		}

		GetComponent<Text> ().text = formattedMessages;
	}

	public class MessageItem
	{
		public string message;
		public Color color = default(Color);
	}
}
