using UnityEngine;

[System.Serializable]
public class PlayerWeapon {
	public string name = "Glock";

	public int damage = 10;
	public float range = 100f;

	public float fireRate = 0f;

	public float spread = 0f;

	public GameObject weaponGFX;
	public AudioClip firstShot;
	public AudioClip[] midShots;
	public AudioClip tailSound;


}
