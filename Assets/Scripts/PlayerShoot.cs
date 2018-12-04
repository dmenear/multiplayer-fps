using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";
	
	private PlayerWeapon currentWeapon;

	private int accuracyDiv;
	private bool firstShot;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private AudioSource playerAudio;
	private WeaponManager weaponManager;
	private PlayerManager playerManager;

	private string hitEffect;

	// Use this for initialization
	void Start () {
		if (cam == null) {
			Debug.LogError("PlayerShoot: No camera referenced!");
			this.enabled = false;
		}
		playerAudio = GetComponent<AudioSource>();
		weaponManager = GetComponent<WeaponManager>();
		playerManager = GetComponent<PlayerManager>();
	}
	
	// Update is called once per frame
	void Update () {
		currentWeapon = weaponManager.GetCurrentWeapon();

		if(!PauseMenu.isOn){
			if (currentWeapon.fireRate <= 0f){
				if (Input.GetButtonDown("Fire1")){
					Shoot();
				}
			} else{
				if (Input.GetButtonDown("Fire1")){
					accuracyDiv = 5;
					firstShot = true;
					InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
				} else if(Input.GetButtonUp("Fire1")){
					CancelInvoke("Shoot");
				}
			}
		} else if(IsInvoking("Shoot")){
			CancelInvoke("Shoot");
		}
	}

	public void stopShooting(){
		if(IsInvoking("Shoot")){
			CancelInvoke("Shoot");
		}
	}

	[Command]
	void CmdOnShoot(){
		RpcDoShootEffect();
	}

	[ClientRpc]
	void RpcDoShootEffect(){
		weaponManager.GetCurrentGraphics().muzzleFlash.Play();
		if(firstShot){
			firstShot = false;
			playerAudio.PlayOneShot(currentWeapon.firstShot, 0.25f);
			playerAudio.PlayOneShot(currentWeapon.tailSound, 0.25f);
		} else{
			playerAudio.PlayOneShot(currentWeapon.midShots[Random.Range(0, currentWeapon.midShots.Length)], 0.25f);
			playerAudio.PlayOneShot(currentWeapon.tailSound, 0.25f);
		}
	}

	[Command]
	void CmdOnHit(Vector3 pos, Vector3 normal, string type){
		RpcDoHitEffect(pos, normal, type);
	}

	[ClientRpc]
	void RpcDoHitEffect(Vector3 pos, Vector3 normal, string type){
		if(type == "metal"){
			GameObject hitEffect = (GameObject)Instantiate (weaponManager.GetCurrentGraphics().metalHitEffectPrefab, pos, Quaternion.LookRotation(normal));
			Destroy(hitEffect, 2f);
		} else if(type == "brick"){
			GameObject hitEffect = (GameObject)Instantiate (weaponManager.GetCurrentGraphics().brickHitEffectPrefab, pos, Quaternion.LookRotation(normal));
			Destroy(hitEffect, 2f);
		}
	}

	[Client]
	void Shoot () {
		if (!isLocalPlayer){
			return;
		}

		CmdOnShoot();

		RaycastHit hit;
		Vector3 shootDirection = cam.transform.forward;
		float currentSpread = currentWeapon.spread;
	
		if (accuracyDiv > 0){
			currentSpread /= accuracyDiv;
			accuracyDiv -= 1;
		}
		shootDirection.x += Random.Range(-currentSpread, currentSpread);
		shootDirection.y += Random.Range(-currentSpread, currentSpread);
		shootDirection.z += Random.Range(-currentSpread, currentSpread);
		if (Physics.Raycast(cam.transform.position, shootDirection, out hit, currentWeapon.range, mask)){
			if (hit.collider.tag == PLAYER_TAG){
				CmdPlayerShot(hit.collider.name, currentWeapon.damage);
				hitEffect = "metal";
			} else{
				hitEffect = "brick";
			}

			CmdOnHit(hit.point, hit.normal, hitEffect);
		}
	}

	[Command]
	void CmdPlayerShot(string playerID, int damage){
		Debug.Log (playerID + " has been shot.");

		PlayerManager player = GameManager.GetPlayer(playerID);
		player.RpcTakeDamage(damage);
	}
}
