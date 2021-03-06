using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This script controls the character's animation by passing velocity values and other information ('isGrounded') to an animator component;
	public class AnimationControl : MonoBehaviour {
		private Controller controller;
		private Animator animator;
		private Transform animatorTransform;
		private Transform tr;

		//Whether the character is using the strafing blend tree;
		public bool useStrafeAnimations = false;

		private float smoothingFactor = 0.8f;
		private Vector3 oldMovementVelocity = Vector3.zero;

		//Setup;
		private void Awake () {
			controller = GetComponent<Controller>();
			animator = GetComponentInChildren<Animator>();
			animatorTransform = animator.transform;

			tr = transform;
		}

		//OnEnable;
		private void OnEnable()
		{
			//Connect events to controller events;
			controller.OnLand += OnLand;
			controller.OnJump += OnJump;
		}

		//OnDisable;
		private void OnDisable()
		{
			//Disconnect events to prevent calls to disabled gameobjects;
			controller.OnLand -= OnLand;
			controller.OnJump -= OnJump;
		}
		
		//Update;
		private void Update () {

			//Get controller velocity;
			Vector3 _velocity = controller.GetVelocity();

			//Split up velocity;
			Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, tr.up);
			Vector3 _verticalVelocity = _velocity - _horizontalVelocity;

			//Smooth horizontal velocity for fluid animation;
			_horizontalVelocity = Vector3.Lerp(oldMovementVelocity, _horizontalVelocity, smoothingFactor);
			oldMovementVelocity = _horizontalVelocity;

			animator.SetFloat("VerticalSpeed", _verticalVelocity.magnitude * VectorMath.GetDotProduct(_verticalVelocity.normalized, tr.up));
			animator.SetFloat("HorizontalSpeed", _horizontalVelocity.magnitude);

			//If animator is strafing, split up horizontal velocity;
			if(useStrafeAnimations)
			{
				Vector3 _localVelocity = animatorTransform.InverseTransformVector(_horizontalVelocity);
				animator.SetFloat("ForwardSpeed", _localVelocity.z);
				animator.SetFloat("StrafeSpeed", _localVelocity.x);
			}

			//Pass values to animator;
			animator.SetBool("IsGrounded", controller.IsGrounded());
			animator.SetBool("IsStrafing", useStrafeAnimations);
		}


		private void OnLand(Vector3 _v)
		{
			animator.SetTrigger("OnLand");
		}

		private void OnJump(Vector3 _v)
		{
			
		}
	}
}
