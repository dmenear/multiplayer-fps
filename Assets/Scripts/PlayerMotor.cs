using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	public void Move(Vector3 vel) {
		velocity = vel;
	}

	public void Rotate(Vector3 rot) {
		rotation = rot;
	}

	public void RotateCamera(float camRotX) {
		cameraRotationX = camRotX;
	}

	public void ApplyThruster(Vector3 tForce) {
		thrusterForce = tForce;
	}

	void FixedUpdate () {
		PerformMovement();
		PerformRotation();
	}

	void PerformMovement () {
		if (velocity != Vector3.zero){
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}
		if (thrusterForce != Vector3.zero){
			rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	void PerformRotation (){
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		if (cam != null){
			//Set rotation and clamp it
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

			//Apply rotation to transform of camera
			cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
		}
	}

}
