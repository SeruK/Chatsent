using UnityEngine;
using System.Collections;

[RequireComponent( typeof(Animator) )]
[RequireComponent( typeof(CharacterController) )]
public class IMVUMove : MonoBehaviour {
	public AudioSource footSource;
	public AudioClip footstepClip;
	public float moveSpeed = 4.0f;
	public float backwardSpeed = 2.0f;
	public float mouseSpeed = 2.0f;

	private Animator animator;
	private CharacterController charController;

	private float animatorSpeed;
	private Vector3 lastPosition;
	private Vector3 currentMovement;

	private void OnEnable() {
		animator = GetComponent<Animator>();
		charController = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() {
//		if( m_PhotonView.isMine == true ) {
			ResetSpeedValues();

			UpdateMovement();

			MoveCharacterController();
			ApplyGravityToCharacterController();

//			ApplySynchronizedValues();
//		}

		UpdateAnimation();
	}

	private void ResetSpeedValues() {
		currentMovement = Vector3.zero;
	}

	private void UpdateMovement() {
		transform.Rotate( Vector3.up, Input.GetAxis( "Mouse X" ) * mouseSpeed );
		//Camera.main.transform.RotateAround( Camera.main.transform.position, Vector3.forward, Input.GetAxis( "Mouse Y" ) * mouseSpeed );

		Vector3 dir = Vector3.zero;

		float hori = Input.GetAxisRaw( "Horizontal" );
		if( !Mathf.Approximately( hori, 0.0f ) ) {
			dir += Vector3.right * Mathf.Sign( hori );
		}

		float vert = Input.GetAxisRaw( "Vertical" );
		if( !Mathf.Approximately( vert, 0.0f ) ) {
			dir += Vector3.forward * Mathf.Sign( vert );
		}

		bool backwards = Vector3.Dot( Vector3.forward, dir ) < 0.0f;

		currentMovement = transform.TransformDirection( dir.normalized ) * ( backwards ? backwardSpeed : moveSpeed );
	}

	private void MoveCharacterController() {
		charController.Move( currentMovement * Time.deltaTime );
	}

	private void ApplyGravityToCharacterController() {
		charController.Move( Physics.gravity * Time.deltaTime );
	}

	private void UpdateAnimation() {
		Vector3 movementVector = transform.position - lastPosition;

		Debug.DrawLine( Vector3.up * 0.5f + transform.position,
			Vector3.up * 0.5f +transform.position + movementVector.normalized * 1.0f );

		float speed = Vector3.Dot( movementVector.normalized, transform.forward );
		float direction = Vector3.Dot( movementVector.normalized, transform.right );

		if( Mathf.Abs( speed ) < 0.2f ) {
			speed = 0f;
		}

		if( speed > 0.6f ) {
			speed = 1f;
		}

		if( speed >= 0f ) {
			if( Mathf.Abs( direction ) > 0.7f ) {
				speed = 1f;
			}
		}

		animatorSpeed = Mathf.MoveTowards( animatorSpeed, speed, Time.deltaTime * 5f );

		animator.SetFloat( "Speed", animatorSpeed);
		animator.SetFloat( "Direction", direction );

		lastPosition = transform.position;
	}

	private void Move( float speed, Vector3 dir ) {
		transform.position += speed * dir;
	}

	private void JumpEvent() {
		Debug.Log( "jumpevent" );
	}

	private void FootstepEvent() {
		if( footSource != null ) {
			footSource.PlayOneShot( footstepClip );
		}
	}
}
