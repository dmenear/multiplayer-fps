using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	RectTransform thrusterFuelFill;
	[SerializeField]
	Text healthVal;
	[SerializeField]
	GameObject pauseMenu;

	private PlayerController controller;
	private PlayerManager manager;

	void Start(){
		PauseMenu.isOn = false;
	}

	public void SetController(PlayerController c){
		controller = c;
	}

	public void SetManager(PlayerManager m){
		manager = m;
	}

	void SetFuelAmount(float amount){
		thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
	}

	void SetHealth(int amount){
		healthVal.GetComponent<Text>().text = amount.ToString();
	}

	void Update(){
		SetFuelAmount(controller.getThrusterFuelAmount());
		SetHealth(manager.GetPlayerHealth());

		if(Input.GetKeyDown(KeyCode.Escape)){
			TogglePauseMenu();
		}
	}

	void TogglePauseMenu(){
		if(pauseMenu.activeSelf){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.isOn = pauseMenu.activeSelf;
	}

}
