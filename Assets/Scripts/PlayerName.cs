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
		if(PlayerPrefs.HasKey("PlayerName") && PlayerPrefs.GetString("PlayerName") != "" && PlayerPrefs.GetString("PlayerName") != null){
			inputPlayerName.text = PlayerPrefs.GetString("PlayerName");
		} else{
			inputPlayerName.text = "Player";
			PlayerPrefs.SetString("PlayerName", inputPlayerName.text);
		}
	}
	
	public void SetPlayerName(string newName){
		if(newName != ""){
			PlayerPrefs.SetString("PlayerName", newName);
		} else{
			PlayerPrefs.SetString("PlayerName", "Player");
		}
	}
}
