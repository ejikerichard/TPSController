using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement3D : GenericBehaviour
{
    public CharacterStates states;

    [SerializeField]
    public Vector3 croucCamPivotOffset = new Vector3(0, 1, 0);
    public Vector3 crouchCamOffset = new Vector3(0.8f, 0.01f, -1.1f);
    public string crouchBool = "IsCrouching";
    public string crouchTag = "AutoCrouch";
    public string groundedBool = "IsGrounded";
    public string distanceFloat = "GroundDistance";
    public bool isCrouch, isClimb, isClimbing;

    [SerializeField]
    public float anim_walkSpeed = 0.15f;
    public float anim_runSpeed = 1.0f;
    public float movementSpeed;
    public float runSpeed;
    public float walkSpeed;
    private float forward;
    private float strafe;
    public float speed;
    public float speeDampTime;
    public float speedSeeker;
    public float movementSpeedSeeker;


    void Start(){
        Init();
    }

    void Init(){
        behaviourController.SubscribeBehaviour(this);
        behaviourController.RegisterDefaultBehaviour(this.behaviourCode);
        states = GetComponent<CharacterStates>();

        //speedSeeker = anim_walkSpeed;

    }
    public override void LocalFixedUpdate(){

        Animate(behaviourController.GetH, behaviourController.GetV);
        GroundCheck();
    }


    #region Animations

    public void Animate(float horizontal, float vertical){

        Rotating(horizontal, vertical);

        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, anim_walkSpeed, anim_runSpeed);
        speed *= speedSeeker;

        movementSpeedSeeker += Input.GetAxis("Mouse ScrollWheel") * 80f;
        movementSpeedSeeker = Mathf.Clamp(movementSpeedSeeker, walkSpeed, runSpeed);
        movementSpeed = movementSpeedSeeker;


        if (speedSeeker == anim_runSpeed)
            states.movement = CharacterStates.Movement.RUNNING;
        else if (speedSeeker == anim_walkSpeed)
            states.movement = CharacterStates.Movement.WALK;


        if(!isCrouch){
            behaviourController.GetAnimator.SetFloat(SpeedFloat, speed, speeDampTime, Time.deltaTime);
        }

        if(isCrouch){
            behaviourController.GetAnimator.SetBool(crouchBool, isCrouch);

            Vector2 direction = new Vector2(horizontal, vertical);
            speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
            speedSeeker = Mathf.Clamp(speedSeeker, anim_walkSpeed, anim_runSpeed);
            speed *= speedSeeker;
     

            behaviourController.GetAnimator.SetFloat(CrouchFloat, speed, speeDampTime, Time.deltaTime);

            behaviourController.GetCameraRig.SetTargetOffsets(croucCamPivotOffset, crouchCamOffset);
        }
        else if(!isCrouch){

            behaviourController.GetAnimator.SetBool(crouchBool, isCrouch);
            behaviourController.GetAnimator.SetFloat(CrouchFloat, 0);
            behaviourController.GetCameraRig.ResetTargetOffsets();
            behaviourController.GetCameraRig.ResetMaxVerticalAngle();
        }
    }

    public void MoveCharacter(float vertical,float horizontal){
        if(states.movement == CharacterStates.Movement.RUNNING || states.movement == CharacterStates.Movement.WALK){

            Vector3 forward = behaviourController.camTransform.TransformDirection(Vector3.forward);

            // Player is moving on ground, Y component of camera facing is not relevant.
            forward.y = 0.0f;
            forward = forward.normalized;

            // Calculate target direction based on camera forward and direction key.
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            Vector3 targetDirection;
            targetDirection = forward * vertical + right * horizontal;

            behaviourController.mybody.velocity = targetDirection * movementSpeed;
        }
    }

    private void OnAnimatorMove(){
        MoveCharacter(Input.GetAxis(UserInput.Instance.input.verticalAxis), Input.GetAxis(UserInput.Instance.input.horizontalAxis));
    }

    void GroundCheck(){
        if(behaviourController.customAction != true){
            behaviourController.GetAnimator.SetFloat(distanceFloat, behaviourController.groundDistance);
        }

        if(behaviourController.isGrounded && behaviourController.customAction != true){
            behaviourController.GetAnimator.SetBool(groundedBool, behaviourController.isGrounded);
        }
        else if(!behaviourController.isGrounded && behaviourController.customAction != true){
            behaviourController.GetAnimator.SetBool(groundedBool, behaviourController.isGrounded);
        }
    }
    #endregion

    #region CharacterRotation
    public Vector3 Rotating(float horizontal, float vertical){

        // Get camera forward direction, without vertical component.
        Vector3 forward = behaviourController.camTransform.TransformDirection(Vector3.forward);

        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Calculate target direction based on camera forward and direction key.
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;

        // Lerp current direction to calculated target  .
        if((behaviourController.IsMoving() && targetDirection != Vector3.zero)){
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviourController.GetRigidbody.rotation, targetRotation, behaviourController.turningSmoothing);
            behaviourController.GetRigidbody.MoveRotation(newRotation);
            behaviourController.SetLastDirection(targetDirection);
        }
        // If idle, Ignore current camera facing and consider last moving direction.
        if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9)){
            behaviourController.Repositioning();
        }

        return targetDirection;
    }
    #endregion


    #region TriggerChecker
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag(crouchTag)){
            isCrouch = true;    
        }

        if(other.gameObject.CompareTag("CombatArena")){
            states.locomotions = CharacterStates.Locomotions.CombatLocomotion;
        }
    }
    void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag(crouchTag)){
            isCrouch = true;
        }
        if(other.gameObject.CompareTag("CombatArena")){
            states.locomotions = CharacterStates.Locomotions.CombatLocomotion;
        }
    }
    void OnTriggerExit(Collider other){
        isCrouch = false;
        states.locomotions = CharacterStates.Locomotions.FreeLocomotion;
    }
    #endregion
}
