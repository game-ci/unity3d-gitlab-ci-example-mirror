using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// This class contains all basic setup and common functions used by all other player behaviours, and also manages wich behaviour is active.
public class BasicBehaviour : MonoBehaviour
{
	public Transform playerCamera;                 // Reference to the camera that focus the player.
	public float turnSmoothing = 3.0f;             // Speed of turn when moving to match camera facing.
	public float sprintFOV = 100f;                 // the FOV to use on the camera when player is sprinting.

	private float h;                                // Horizontal Axis.
	private float v;                                // Vertical Axis.
	private int currentBehaviour;                   // Reference to the current player behaviour.
	private int defaultBehaviour;                   // The default behaviour of the player when any other is not active.
	private Vector3 lastDirection;                 // Last direction the player was moving.
	private Animator anim;                         // Reference to the Animator component.
	private ThirdPersonOrbitCam camScript;         // Reference to the third person camera script.
	private bool sprint;                           // Boolean to determine whether or not the player activated the sprint mode.
	private int hFloat;                            // Animator variable related to Horizontal Axis.
	private int vFloat;                            // Animator variable related to Vertical Axis.
	private List<GenericBehaviour> behaviours;     // The list containing all the enabled player behaviours.
	private Rigidbody rbody;                       // Reference to the player's rigidbody.

	// Current horizontal and vertical axes.
	public float GetH { get { return h; } }
	public float GetV { get { return v; } }

	void Awake ()
	{
		// Set up the references.
		behaviours = new List<GenericBehaviour> ();
		anim = GetComponent<Animator> ();
		hFloat = Animator.StringToHash("H");
		vFloat = Animator.StringToHash("V");
		camScript = playerCamera.GetComponent<ThirdPersonOrbitCam> ();
		rbody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		// Store the input axes.
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		// Set the input axes on the Animator Controller.
		anim.SetFloat(hFloat, h);
		anim.SetFloat(vFloat, v);

		// Toggle sprint by input.
		sprint = Input.GetButton ("Sprint");

		// Set the correct camera FOV.
		if(isSprinting())
		{
			camScript.SetFOV(sprintFOV);
		}
		else
		{
			camScript.ResetFOV();
		}
	}

	void FixedUpdate()
	{
		// Ensure the camera will be back to original setup when no behaivour is active.
//		camScript.ResetTargetOffsets ();
//		camScript.ResetMaxVerticalAngle ();

		// Call the active behaviour.
		bool isAnyBehaviourActive = false;
		foreach (var behaviour in behaviours)
		{
			if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
			{
				isAnyBehaviourActive = true;
				behaviour.LocalFixedUpdate ();
			}
		}

		// Ensure the player is standed on floor if no behaivour is active.
		if (!isAnyBehaviourActive)
		{
			rbody.useGravity = true;
			Repositioning ();
		}
	}

	// Puts a new behaviour on the monitored behaviours list.
	public void SubscribeBehaviour(GenericBehaviour behaviour)
	{
		behaviours.Add (behaviour);
	}

	// Set the default player behaviour.
	public void RegisterDefaultBehaviour(int behaviourCode)
	{
		defaultBehaviour = behaviourCode;
		currentBehaviour = behaviourCode;
	}

	// Attempt to set a player behaviour as the active one.
	public void RegisterBehaviour(int behaviourCode)
	{
		if (currentBehaviour == defaultBehaviour)
		{
			currentBehaviour = behaviourCode;
		}
	}

	// Attempt to deactivate a player behaviour and return to the default one.
	public void UnregisterBehaviour(int behaviourCode)
	{
		if (currentBehaviour == behaviourCode)
		{
			currentBehaviour = defaultBehaviour;
		}
	}

	// Check if the active behaviour is the passed one.
	public bool IsCurrentBehaviour(int behaviourCode)
	{
		return this.currentBehaviour == behaviourCode;
	}

	// Check if player is sprinting.
	public virtual bool isSprinting()
	{
		return sprint && IsMoving() && CanSprint();
	}

	// Check if player can sprint (all behaviours must allow).
	public bool CanSprint()
	{
		foreach (var behaviour in behaviours)
		{
			if (!behaviour.AllowSprint ())
				return false;
		}
		return true;
	}

	// Check if the player is moving on the horizontal plane.
	public bool IsHorizontalMoving()
	{
		return Mathf.Abs(h) > 0.1;
	}

	// Check if the player is moving.
	public bool IsMoving()
	{
		return Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;
	}

	// Set the last player direction of facing.
	public void SetLastDirection(Vector3 direction)
	{
		lastDirection = direction;
	}

	// Put the player on a standing up position based on last direction faced.
	public void Repositioning()
	{
		if(lastDirection != Vector3.zero)
		{
			lastDirection.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation (lastDirection);
			Quaternion newRotation = Quaternion.Slerp(rbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
			rbody.MoveRotation (newRotation);
		}
	}
}

// This is the base class for all player behaviours, any custom behaviour must inherit from this.
public abstract class GenericBehaviour : MonoBehaviour
{
	protected Rigidbody rbody;                     // Reference to the player's rigidbody.
	protected Animator anim;                       // Reference to the Animator component.
	protected int speedFloat;                      // Speed parameter on the Animator.
	protected BasicBehaviour behaviourManager;     // Reference to the basic behaviour manager.
	protected ThirdPersonOrbitCam camScript;       // Reference to the third person camera script.
	protected int behaviourCode;                   // The code that identifies a behaviour.
	protected bool canSprint;                      // Boolean to store if the behaviour allows the player to sprint.
	protected float distToGround;                   // Actual distance to ground.

	void Awake()
	{
		// Set up the references.
		anim = GetComponent<Animator> ();
		rbody = GetComponent<Rigidbody> ();
		behaviourManager = GetComponent<BasicBehaviour> ();
		camScript = behaviourManager.playerCamera.GetComponent<ThirdPersonOrbitCam> ();
		speedFloat = Animator.StringToHash("Speed");
		canSprint = true;

		// Set the behaviour code based on the inheriting class.
		behaviourCode = this.GetType().GetHashCode();
		distToGround = GetComponent<Collider>().bounds.extents.y;
	}

	// Protected, virtual functions can be overridden by inheriting classes.
	// Here the custom active behaviour will control the player actions.
	public abstract void LocalFixedUpdate ();

	// Get the behaviour code.
	public int GetBehaviourCode()
	{
		return behaviourCode;
	}

	// Check if the behaviour allows sprinting.
	public bool AllowSprint()
	{
		return canSprint;
	}

	// Function to tell whether or not the player is on ground.
	public bool IsGrounded() {
		return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
	}
}
