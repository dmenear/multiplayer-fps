using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon;

	private PlayerWeapon currentWeapon;

	private WeaponGraphics currentGraphics;

	// Use this for initialization
	void Start () {
		EquipWeapon(primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon(){
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentGraphics(){
		return currentGraphics;
	}

	void EquipWeapon(PlayerWeapon newWeapon){
		currentWeapon = newWeapon;

		GameObject weaponInstance = Instantiate(newWeapon.weaponGFX, weaponHolder.position, weaponHolder.rotation);
		weaponInstance.transform.SetParent(weaponHolder);

		currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();
		if (currentGraphics == null){
			Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponInstance.name);
		}

		if(isLocalPlayer){
			Util.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
