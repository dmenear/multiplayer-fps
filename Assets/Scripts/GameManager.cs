using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager singleton;

	public MatchSettings matchSettings;

	[SerializeField]
	private GameObject sceneCamera;


	void Awake (){
		if (singleton != null){
			Debug.LogError("More than one GameManager in scene.");
		} else{
			singleton = this;
		}
	}

	public void setSceneCameraActive(bool isActive){
		if (sceneCamera == null){
			return;
		} else{
			sceneCamera.SetActive(isActive);
		}
	}

	#region Player Tracking


	private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

	public static void RegisterPlayer(string netID, PlayerManager player){
		players.Add(netID, player);
		player.transform.name = netID;
	}

	public static void UnregisterPlayer(string netID){
		players.Remove(netID);
	}

	public static PlayerManager GetPlayer (string netID){
		return players[netID];
	}
	
	// void OnGUI(){
	// 	GUILayout.BeginArea (new Rect(200, 200, 200, 500));
	// 	GUILayout.BeginVertical();

	// 	foreach (string playerID in players.Keys) {
	// 		GUILayout.Label (playerID + " - " + players[playerID].transform.name);
	// 	}

	// 	GUILayout.EndVertical();
	// 	GUILayout.EndArea();
	// }

	#endregion
}
