using System;
using UnityEngine;

[AddComponentMenu("Player/Fps Movement")]
[RequireComponent(typeof(GroundCheck))]
public class FpsMovement : MonoBehaviour
{
   [Header("Movement Settings")]
   public float acceleration = 100;
   public float friction = 7;
   [Space]
   public float jumpForce = 8;
   public float jumpTimeOut = .5f;
   [Space] 
   public float extraGravity = 1;

   [Header("Camera Settings")]
   public Vector2 clampYLook = new Vector2(-90, 90);
   public Transform cameraHolder;
   
   [Header("References")]
   public Transform orientation;
   public Transform cameraPosition;
   
   private Vector2 _moveDirection;
   private Vector2 _mouse;
   
   private Vector3 _currentCameraPosition;

   private bool _jumping;
   private bool _canJump = true;
   
   private Rigidbody _rigidbody;
   private GroundCheck _groundCheck;
   
   private static FpsMovement _instance;

   #region Unity Functions

   private void Awake()
   {
      _instance = this;
   }

   private void Start()
   {
      _rigidbody = GetComponent<Rigidbody>();
      _groundCheck = GetComponent<GroundCheck>();
   }

   private void Update()
   {
      Inputs();
      Look();

      if (_jumping && _groundCheck.Grounded() && _canJump)
      {
         _canJump = false;
         Invoke(nameof(ResetJump), jumpTimeOut);
         Jump();
      }
         
      _currentCameraPosition = cameraPosition.position;
   }

   private void FixedUpdate()
   {
      Move();
      Friction();
      ExtraGravity();
   }

   private void LateUpdate()
   {
      cameraHolder.position = _currentCameraPosition;
   }

   #endregion
   
   private void Inputs()
   {
      //Movement
      _moveDirection = InputManager.MovementDirection();
      
      //Mouse
      _mouse += InputManager.Mouse();
      _mouse.y = Mathf.Clamp(_mouse.y, clampYLook.x, clampYLook.y);
      
      //Jump
      _jumping = InputManager.Jump() == 1;
   }

   private void Look()
   {
      cameraHolder.rotation = Quaternion.Euler(_mouse.y, _mouse.x, 0);
      orientation.rotation = Quaternion.Euler(0, _mouse.x, 0);
   }

   private void Move()
   {
      Vector3 forward = orientation.forward;
      Vector3 right = orientation.right;
      Vector3 desiredInputDirection = (_moveDirection.x * right + _moveDirection.y * forward).normalized;

      Vector3 force = desiredInputDirection * acceleration;
      
      _rigidbody.AddForce(force, ForceMode.Acceleration);
   }

   private void Jump()
   {
      //Zero out the y velocity to make jumps more consistent
      _rigidbody.velocity = GetFlatVelocity();
      
      Vector3 force = jumpForce * orientation.up;
      _rigidbody.AddForce(force, ForceMode.VelocityChange);
   }
   
   private void ResetJump()
   {
      _canJump = true;
   }

   private void Friction()
   {
      Vector3 relativeVelocity = GetVelocityRelativeToLook();
      Vector3 velocity = GetFlatVelocity();
      
      float angle = Vector3.SignedAngle(relativeVelocity, velocity, Vector3.up);
         
      if (_moveDirection.x == 0 && !_groundCheck.Grounded()) relativeVelocity.x = 0;
      if (_moveDirection.y == 0 && !_groundCheck.Grounded()) relativeVelocity.z = 0;
      
      
      Vector3 newVelocity = Quaternion.AngleAxis(angle, Vector3.up) * relativeVelocity;
      newVelocity *= -1;
      
      Vector3 force = newVelocity * friction;
      _rigidbody.AddForce(force, ForceMode.Acceleration);
   }

   private void ExtraGravity()
   {
      Vector3 force = Physics.gravity * extraGravity;
      _rigidbody.AddForce(force, ForceMode.Acceleration);
   }

   #region Static
   public static Vector3 GetFlatVelocity()
   {
      Vector3 velocity = _instance._rigidbody.velocity;
      Vector3 flatVelocity = new Vector3(velocity.x, 0, velocity.z);
      return flatVelocity;
   }

   public static Vector3 GetVelocity()
   {
      Vector3 velocity = _instance._rigidbody.velocity;
      return velocity;
   }
   
   public static Vector3 GetVelocityRelativeToLook() {
      float lookAngle = _instance.orientation.transform.eulerAngles.y;
      Vector3 velocity = GetFlatVelocity();
      float moveAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

      float u = Mathf.DeltaAngle(lookAngle, moveAngle);
      float v = 90 - u;

      float magnitude = velocity.magnitude;
      float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
      float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        
      return new Vector3(xMag, 0, yMag);
   }
   
   public static Transform GetOrientation()
   {
      return _instance.orientation;
   }
   #endregion

   private void OnDrawGizmos()
   {
      Vector3 position = orientation.position;
      Vector3 forward = orientation.forward;
      Vector3 right = orientation.right;
      
      Gizmos.color = Color.blue;
      Gizmos.DrawRay(position, forward);
      Gizmos.color = Color.red;
      Gizmos.DrawRay(position, right);
   }
}
