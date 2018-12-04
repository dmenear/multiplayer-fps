using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[Header("Player Movement")]
	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;
	[SerializeField]
	private float thrusterForce = 1000f;
	[SerializeField]
	private float thrusterFuelBurnSpeed = 0.25f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.15f;
	[SerializeField]
	private AudioSource thrusterAudio;

	[Header("Spring Settings")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	private float defaultThrusterRegen;
	private float thrusterFuelAmount = 1f;

	private bool isMoving = false;
	private bool isThrusting = false;
	private float tPitch;

	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	// Use this for initialization
	void Start () {
		motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint>();
		animator = GetComponent<Animator>();

		SetJointSettings(jointSpring);
		defaultThrusterRegen = thrusterFuelRegenSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		//Calculate movement velocity as 3D vector
		float xMov = Input.GetAxis("Horizontal");
		float zMov = Input.GetAxis("Vertical");
		if(PauseMenu.isOn){
			xMov = 0f;
			zMov = 0f;
		}

		if(xMov != 0f || zMov != 0f){
			isMoving = true;
			thrusterFuelRegenSpeed = defaultThrusterRegen;
		} else{
			isMoving = false;
			thrusterFuelRegenSpeed = defaultThrusterRegen + 0.1f;
		}

		if(Input.GetButton("Jump") && thrusterFuelAmount >= 0.01f && !PauseMenu.isOn){
			isThrusting = true;
		} else{
			isThrusting = false;
		}

		tPitch = 1.0f;
		if(isMoving || isThrusting){
			if (isMoving) tPitch += 0.25f;
			if (isThrusting) tPitch += 0.35f;
		}
		if(thrusterAudio.pitch < tPitch){
			thrusterAudio.pitch += (0.02f);
		} else if (thrusterAudio.pitch > tPitch){
			thrusterAudio.pitch -= (0.02f);
		}
		if(PauseMenu.isOn){
			tPitch = 1.0f;
		}

		Vector3 movHorizontal = transform.right * xMov;
		Vector3 movVertical = transform.forward * zMov;

		//Final movement vector
		Vector3 velocity = (movHorizontal + movVertical) * speed;

		//Animate movement
		animator.SetFloat("ForwardVelocity", zMov);

		motor.Move(velocity);

		//Calculate rotation as 3D vector
		float yRot = Input.GetAxisRaw("Mouse X");
		if(PauseMenu.isOn){
			yRot = 0f;
		}

		Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

		//Apply rotation
		motor.Rotate(rotation);

		//Calculate camera rotation as 3D vector
		float xRot = Input.GetAxisRaw("Mouse Y");
		if(PauseMenu.isOn){
			xRot = 0f;
		}

		float cameraRotationX = xRot * lookSensitivity;

		//Apply rotation
		motor.RotateCamera(cameraRotationX);

		//Detect distance to ground below
		RaycastHit below;
		if (Physics.Raycast(transform.position, -transform.up, out below, 1000f)){
			joint.connectedAnchor = new Vector3(0f, below.point.y + 1.1f, 0f);
		}

		//Calculate thruster force based on user input
		Vector3 tForce = Vector3.zero;
		if (Input.GetButton("Jump") && thrusterFuelAmount > 0f && !PauseMenu.isOn) {
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
			if (thrusterFuelAmount >= 0.01f){
				tForce = Vector3.up * thrusterForce;
				SetJointSettings(0f);
			}

		} else{
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings(jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

		if(PauseMenu.isOn){
			tForce = Vector3.zero;
		}

		//Apply thruster force
		motor.ApplyThruster(tForce);
	}

	private void SetJointSettings(float jSpring){
		joint.yDrive = new JointDrive {
			positionSpring = jSpring,
			maximumForce = jointMaxForce
		};
	}

	public float getThrusterFuelAmount(){
		return thrusterFuelAmount;
	}
}
