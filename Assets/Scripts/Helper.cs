using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class Helper {
	
	// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
	public static string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static string FloatToDezimalWithlength(float orgNumber, int length){
		float intNumber = orgNumber * 100f;
		int intNumberLength = intNumber.ToString("0.##").Length;

		string formattedMessages = "";
		string hexColorGrey = Helper.ColorToHex (Color.gray);
		formattedMessages = "<color=#" + hexColorGrey + ">";
		for(int i = 0; i < (length-intNumberLength); i++){
			formattedMessages += "0";
		}
		formattedMessages += "</color>";
		if(orgNumber > 1){
			formattedMessages += orgNumber.ToString("0.00");
		} else {
			formattedMessages += "." + intNumber;
		}
		return formattedMessages;
	}

	public static void colorSprites (Transform newObject, Color _color){
		for (int childIndex = 0; childIndex < newObject.childCount; childIndex ++) {
			Transform child = newObject.GetChild(childIndex);
			if(child.GetComponent<SpriteRenderer> () != null)
				child.GetComponent<SpriteRenderer> ().color = _color;
			if(child.GetComponent<Text> () != null)
				child.GetComponent<Text> ().color = _color;
			
			if(child.childCount > 0)
				colorSprites(child, _color);
		}
		
		if(newObject.GetComponent<SpriteRenderer> () != null)
			newObject.GetComponent<SpriteRenderer> ().color = _color;
		if(newObject.GetComponent<Text> () != null)
			newObject.GetComponent<Text> ().color = _color;
	}
}
