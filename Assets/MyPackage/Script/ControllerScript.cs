using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class ControllerScript : MonoBehaviour
	{
		[SerializeField] private bool m_IsWalking;
		[SerializeField] private float m_WalkSpeed;
		[SerializeField] private float m_RunSpeed;
		public float m_RotateSpeed;
		[SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
		[SerializeField] private float m_JumpSpeed;
		[SerializeField] private float m_StickToGroundForce;
		[SerializeField] private float m_GravityMultiplier;
		[SerializeField] private MouseLook m_MouseLook;
		[SerializeField] private bool m_UseFovKick;
		[SerializeField] private FOVKick m_FovKick = new FOVKick();
		[SerializeField] private bool m_UseHeadBob;
		//[SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
		[SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
		[SerializeField] private float m_StepInterval;
		[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
		[SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
		[SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.


		private bool m_Jump;
		private float m_YRotation;
		private Vector2 m_Input;
		private Vector3 m_MoveDir = Vector3.zero;
		private CharacterController m_CharacterController;
		private CollisionFlags m_CollisionFlags;
		private bool m_PreviouslyGrounded;
		private Vector3 m_OriginalCameraPosition;
		//private float m_StepCycle;
		//private float m_NextStep;
		private bool m_Jumping;
		//private AudioSource m_AudioSource;

		public bool ManualMode = false;
		public bool SpeedMode = false;

		// Use this for initialization
		private void Start()
		{
			m_CharacterController = GetComponent<CharacterController>();
			//m_StepCycle = 0f;
			//m_NextStep = m_StepCycle/2f;
			m_Jumping = false;
			//m_AudioSource = GetComponent<AudioSource>();

		}


		// Update is called once per frame
		private void Update()
		{
			//RotateView();
			// the jump state needs to read here to make sure it is not missed
			if (!m_Jump)
			{
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			}

			if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
			{
				StartCoroutine(m_JumpBob.DoBobCycle());
				m_MoveDir.y = 0f;
				m_Jumping = false;
			}
			if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
			{
				m_MoveDir.y = 0f;
			}

			m_PreviouslyGrounded = m_CharacterController.isGrounded;


		}


		private void FixedUpdate()
		{
			float speed;
			GetInput(out speed);
			// always move along the camera forward as it is the direction that it being aimed at
			Vector3 desiredMove = transform.forward*m_Input.y;

			// get a normal for the surface that is being touched to move along it
			//RaycastHit hitInfo;
			//Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
			//	m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
			//desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

			m_MoveDir.x = desiredMove.x*speed;
			m_MoveDir.z = desiredMove.z*speed;


			if (m_CharacterController.isGrounded)
			{
				m_MoveDir.y = -m_StickToGroundForce;

			}
			else
			{
				m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
			}
			m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

			this.transform.Rotate(new Vector3(0, m_Input.x*Time.fixedDeltaTime * m_RotateSpeed, 0));

		}


		private void SpeedInput(ref float horizontal, ref float vertical){
			if (ManualMode) {
				horizontal = (Input.GetKey (KeyCode.A) ? -1 : horizontal);
				horizontal = (Input.GetKey(KeyCode.D) ? 1 : horizontal);
				vertical = (Input.GetKey (KeyCode.W) ? 1 : vertical);
				vertical = (Input.GetKey (KeyCode.S) ? -1 : vertical);
			} else if (SpeedMode) {
				horizontal = 1;
				vertical = 1;
			} else {
				horizontal = (InputManager.GetKey (KeyCode.A) ? -1 : horizontal);
				horizontal = (InputManager.GetKey (KeyCode.D) ? 1 : horizontal);
				vertical = (InputManager.GetKey (KeyCode.W) ? 1 : vertical);
				vertical = (InputManager.GetKey (KeyCode.S) ? -1 : vertical);
			}
		}

		private void GetInput(out float speed)
		{
			// Read input
			float horizontal =0;
			float vertical =0;


			SpeedInput (ref horizontal, ref vertical);

			bool waswalking = m_IsWalking;

			speed = m_WalkSpeed;
			m_Input = new Vector2(horizontal, vertical);

			// normalize input if it exceeds 1 in combined length:
			if (m_Input.sqrMagnitude > 1)
			{
				//m_Input.Normalize();
			}

			// handle speed change to give an fov kick
			// only if the player is going to a run, is running and the fovkick is to be used
			if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
			{
				StopAllCoroutines();
				StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
			}
		}

		public float Speed{
			get{ return m_WalkSpeed; }
			set{ m_WalkSpeed = value; }
		}

		public float RotateSpeed{
			get{ return m_RotateSpeed; }
			set{ m_RotateSpeed = value; }
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody body = hit.collider.attachedRigidbody;
			//dont move the rigidbody if the character is on top of it
			if (m_CollisionFlags == CollisionFlags.Below)
			{
				return;
			}

			if (body == null || body.isKinematic)
			{
				return;
			}
			//body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
		}

		public void OnChangeMode(int index){
			switch (index) {
			case 0:
				ManualMode = true;
				SpeedMode = false;
				break;
			case 1:
				ManualMode = false;
				SpeedMode = false;
				break;
			case 2:
				ManualMode = false;
				SpeedMode = true;
				break;
			}
		}
	}
}

