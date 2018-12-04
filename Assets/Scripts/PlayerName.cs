using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerName : MonoBehaviour {

	private InputField inputPlayerName;

	// Use this for initialization
	void Start () {
		inputPlayerName = GetComponent<InputField>();
		PlayerPrefs.SetString("PlayerName", inputPlayerName.text);
	}
	
	public void SetPlayerName(string newName){
		if(newName != ""){
			PlayerPrefs.SetString("PlayerName", newName);
		} else{
			PlayerPrefs.SetString("PlayerName", "Player");
		}
	}
}
