using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

	public delegate void JoinGameDelegate(MatchInfoSnapshot match);
	private JoinGameDelegate joinGameCallback;

	[SerializeField]
	private Text roomNameText;

	private MatchInfoSnapshot match;

	public void Setup(MatchInfoSnapshot _match, JoinGameDelegate _joinGameCallback){
		match = _match;
		joinGameCallback = _joinGameCallback;
		roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
	}

	public void JoinGame(){
		joinGameCallback.Invoke(match);
	}

}
