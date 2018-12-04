using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

	List<GameObject> roomList = new List<GameObject>();

	[SerializeField]
	private Text status;
	[SerializeField]
	private GameObject roomListItemPrefab;
	[SerializeField]
	private Transform roomListParent;

	private NetworkManager networkManager;

	void Start(){
		networkManager = NetworkManager.singleton;
		if(networkManager.matchMaker == null){
			networkManager.StartMatchMaker();
		}

		RefreshRoomList();
	}

	public void RefreshRoomList(){
		ClearRoomList();
		networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
		status.text = "Loading...";
	}

	public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList){
		status.text = "";
		if(!success){
			status.text = "Failed to get list.";
			return;
		}

		foreach (MatchInfoSnapshot match in matchList){
			GameObject roomListItem = Instantiate(roomListItemPrefab);
			roomListItem.transform.SetParent(roomListParent);
			RoomListItem rli = roomListItem.GetComponent<RoomListItem>();
			if(rli != null){
				rli.Setup(match, JoinRoom);
			}
			roomList.Add(roomListItem);
		}

		if (roomList.Count == 0){
			status.text = "No online servers found.";
		}
	}

	void ClearRoomList(){
		for(int i = 0; i < roomList.Count; i++){
			Destroy(roomList[i]);
		}

		roomList.Clear();
	}

	public void JoinRoom(MatchInfoSnapshot match){
		networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		ClearRoomList();
		status.text = "Joining...";
	}

}
