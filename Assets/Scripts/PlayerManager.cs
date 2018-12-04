using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerSetup))]
[RequireComponent(typeof(PlayerShoot))]
public class PlayerManager : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;
	
	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	private bool firstSetup = true;

	private PlayerShoot playerShoot;

	void Start(){
		playerShoot = GetComponent<PlayerShoot>();
	}

	public void SetupPlayer(){
		if(isLocalPlayer){
			GameManager.singleton.setSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		CmdBroadcastNewPlayerSetup();
	}

	void Update(){
		if(isLocalPlayer){
			if(Input.GetKeyDown(KeyCode.K)){
				RpcTakeDamage(9999);
			}
		}
	}

	[Command]
	private void CmdBroadcastNewPlayerSetup(){
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients(){
		if(firstSetup){
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++){
				wasEnabled[i] = disableOnDeath[i].enabled;
			}
			firstSetup = false;
		}
		SetDefaults();
	}

	public int GetPlayerHealth(){
		return currentHealth;
	}

	[ClientRpc]
	public void RpcTakeDamage(int amount){
		if(isDead) return;

		currentHealth -= amount;

		Debug.Log (transform.name + " now has " + currentHealth + " health.");

		if (currentHealth <= 0){
			Die();
		}
	}

	private void Die(){
		isDead = true;

		playerShoot.stopShooting();

		for (int i = 0; i < disableOnDeath.Length; i++){
			disableOnDeath[i].enabled = false;
		}

		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++){
			disableGameObjectsOnDeath[i].SetActive(false);
		}

		//Disable collider
		Collider col = GetComponent<Collider>();
		if (col != null) {
			col.enabled = false;
		}

		GameObject instanceGFX = (GameObject)Instantiate (deathEffect, transform.position, Quaternion.identity);
		Destroy(instanceGFX, 2f);

		//Switch cameras
		if (isLocalPlayer){
			GameManager.singleton.setSceneCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		Debug.Log(transform.name + " is dead.");

		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn(){
		yield return new WaitForSeconds(GameManager.singleton.matchSettings.respawnTime);

		Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;

		yield return new WaitForSeconds(0.25f);

		SetupPlayer();

		Debug.Log(transform.name + " respawned.");
	}

	public void SetDefaults(){
		isDead = false;
		currentHealth = maxHealth;
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath[i].enabled = wasEnabled[i];
		}

		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++){
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		Collider col = GetComponent<Collider>();
		if (col != null) {
			col.enabled = true;
		}
		
	}

}
