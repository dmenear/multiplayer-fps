using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerGraphics;

	[SerializeField]
	GameObject playerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer) {
			DisableComponents();
			AssignRemoteLayer();
		} else {
			//Disable player graphics for local player
			Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			//Create player UI
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			//Configure player UI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null){
				Debug.LogError("No PlayerUI component on PlayerUI prefab.");
			}
			ui.SetController(GetComponent<PlayerController>());
			ui.SetManager(GetComponent<PlayerManager>());
			GetComponent<PlayerManager>().SetupPlayer();
		}
	}

	void AssignRemoteLayer () {
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}

	void DisableComponents () {
		for (int i = 0; i < componentsToDisable.Length; i++){
			componentsToDisable[i].enabled = false;
		}
	}
	
	void OnDisable(){
		Destroy(playerUIInstance);
		GameManager.UnregisterPlayer(GetComponent<NetworkIdentity>().netId.ToString());
		if(isLocalPlayer){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			GameManager.singleton.setSceneCameraActive(true);
		}
	}

	public override void OnStartClient(){
		base.OnStartClient();
		string netID = GetComponent<NetworkIdentity>().netId.ToString();
		PlayerManager player = GetComponent<PlayerManager>();
		GameManager.RegisterPlayer(netID, player);
	}
}
