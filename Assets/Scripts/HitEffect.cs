using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class HitEffect : MonoBehaviour {

	private AudioSource audio;
	public AudioClip[] hitSFX;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		audio.clip = hitSFX[Random.Range(0, hitSFX.Length)];
		audio.Play();
	}
	
}
