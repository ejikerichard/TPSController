using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    public Transform camTransform;
    public float turningSmoothing = 0.06f;

    public enum GroundCheckMethod{
        Low, High
    }

    [SerializeField]
    public float h;
    public float v;
    public int currentBehaviour;
    public int defaultBehaviour;
    public int behaviourLocked;
    public Vector3 lastDirection;
    public Animator animator;
    public CameraRig cameraRig;
    public bool changeFov;
    public int hFloat;
    public int vFloat;
    public List<GenericBehaviour> behaviours;
    public List<GenericBehaviour> overridingbehaviours;
    public Rigidbody mybody;
    public CapsuleCollider capsuleCollider;
    public GroundCheckMethod groundCheckMethod = GroundCheckMethod.High;

    public LayerMask groundLayer = 1 << 0;

    [SerializeField]
    internal float colliderRadius, colliderHeight;
    internal Vector3 colliderCenter;
    public bool isGrounded;
    internal PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;

    public bool isRolling;
    public bool customAction;
    public bool isShooting;
    public bool isJumping;
    public bool lockOn;


    public float groundDistance;
    protected float groundMinDistance = 0.25f;
    protected float groundMaxDistance = 0.5f;
    public float slopeLimit = 75f;
    public float speed;
    public RaycastHit groundHit;

    internal AnimatorStateInfo baseLayerInfo, underBodyInfo, upperBodyInfo, rightArmInfo, leftArmInfo, shotBodyInfo;

    int baseLayer { get { return animator.GetLayerIndex("Base Layer"); } }
    int underBodyLayer { get { return animator.GetLayerIndex("UnderBody"); } }
    int upperBodyLayer { get { return animator.GetLayerIndex("UpperBody"); } }
    int rightArmLayer { get { return animator.GetLayerIndex("RightArm"); } }
    int leftArmLayer { get { return animator.GetLayerIndex("LeftArm"); } }
    int shotBodyLayer { get { return animator.GetLayerIndex("Shot"); } }


    public float GetH { get { return h; } }
    public float GetV { get { return v; } }

    public CameraRig GetCameraRig { get { return cameraRig; } }

    public Rigidbody GetRigidbody { get { return mybody; } }

    public Animator GetAnimator { get { return animator; } }

    public CapsuleCollider GetCapsuleCollider { get { return capsuleCollider; } }

    public int GetDefaultBehaviour { get { return defaultBehaviour; } }

    void Awake(){

        behaviours = new List<GenericBehaviour>();
        overridingbehaviours = new List<GenericBehaviour>();
        animator = GetComponent<Animator>();
        hFloat = Animator.StringToHash("HorizontalInput");
        vFloat = Animator.StringToHash("VerticalInput");     
        cameraRig = camTransform.GetComponent<CameraRig>();
        mybody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        frictionPhysics = new PhysicMaterial();
        frictionPhysics.name = "frictionPhysics";
        frictionPhysics.staticFriction = .25f;
        frictionPhysics.dynamicFriction = .25f;
        frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

        maxFrictionPhysics = new PhysicMaterial();
        maxFrictionPhysics.name = "maxFrictionPhysics";
        maxFrictionPhysics.staticFriction = 1f;
        maxFrictionPhysics.dynamicFriction = 1f;
        maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

        slippyPhysics = new PhysicMaterial();
        slippyPhysics.name = "slippyPhysics";
        slippyPhysics.staticFriction = 0f;
        slippyPhysics.dynamicFriction = 0f;
        slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

        colliderCenter = GetComponent<CapsuleCollider>().center;
        colliderHeight = GetComponent<CapsuleCollider>().height;
        colliderRadius = GetComponent<CapsuleCollider>().radius;

        Collider[] AllColliders = this.GetComponentsInChildren<Collider>();
        for(int i =0; i < AllColliders.Length; i++){
            Physics.IgnoreCollision(capsuleCollider, AllColliders[i]);
        }

        isGrounded = true;
    }

    void Update(){
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");


        animator.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        animator.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        ActionControl();
        LayerControl();
    }
    void FixedUpdate(){
        bool isAnyBehaviourActive = false;
        if(behaviourLocked > 0 || overridingbehaviours.Count == 0){
            foreach(GenericBehaviour behaviour in behaviours)
            {
                isAnyBehaviourActive = true;
                behaviour.LocalFixedUpdate();
            }
        }
        else
        {
            foreach(GenericBehaviour behaviour in overridingbehaviours)
            {
                behaviour.LocalFixedUpdate();
            }
        }

        if(!isAnyBehaviourActive && overridingbehaviours.Count == 0)
        {
            mybody.useGravity = true;

        }
        CheckGround();
    }

    private void LateUpdate(){
        if(behaviourLocked > 0 || overridingbehaviours.Count == 0){
            foreach(GenericBehaviour behaviour in behaviours){
                if(behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        else
        {
            foreach(GenericBehaviour behaviour in overridingbehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }
    //private void OnAnimatorMove()
    //{
    //    if (!this.enabled)
    //        return;

    //    animator.applyRootMotion = true;
    //}
    public void LayerControl(){
        baseLayerInfo = animator.GetCurrentAnimatorStateInfo(baseLayer);
        underBodyInfo = animator.GetCurrentAnimatorStateInfo(underBodyLayer);
        upperBodyInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
        rightArmInfo = animator.GetCurrentAnimatorStateInfo(rightArmLayer);
        leftArmInfo = animator.GetCurrentAnimatorStateInfo(leftArmLayer);
        shotBodyInfo = animator.GetCurrentAnimatorStateInfo(shotBodyLayer);
    }

    public void ActionControl(){

        isShooting = shotBodyInfo.IsName("BowShot");
        customAction = IsAnimatorTag("CustomAction");

        if(customAction){
          //  Debug.Log("ClimbUp");
        }
    }

    public void CheckGround(){

        CheckGroundDistance();

        if(customAction){
            isGrounded = true;
        }

        capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

        if (isGrounded && NotMoving())
            capsuleCollider.material = maxFrictionPhysics;
        else if (isGrounded && NotMoving())
            capsuleCollider.material = frictionPhysics;
        else
            capsuleCollider.material = slippyPhysics;

        bool checkGroundConditions = !isRolling;

        var magVel = (float)System.Math.Round(new Vector3(mybody.velocity.x, 0, mybody.velocity.z).magnitude, 2);
        magVel = Mathf.Clamp(magVel, 0, 1);

        var groundCheckDistance = groundMinDistance;
        if (magVel > 0.25f) groundCheckDistance = groundMaxDistance;

        if(checkGroundConditions){

            if(groundDistance <= 0.05f){
                isGrounded = true;
            }
            else
            {
                if(groundDistance >= groundCheckDistance){
                    isGrounded = false;
                }
            }
        }

    }
    public void CheckGroundDistance(){
        if(capsuleCollider != null){
            float radius = capsuleCollider.radius * 0.9f;
            var dist = 10f;

            Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);

            if (Physics.Raycast(ray2, out groundHit, colliderHeight / 2 + 2f, groundLayer))
                dist = transform.position.y - groundHit.point.y;
            
            if(groundCheckMethod == GroundCheckMethod.High){

                Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if(Physics.SphereCast(ray, radius, out groundHit, capsuleCollider.radius + 2f, groundLayer)){

                    if (dist > (groundHit.distance - capsuleCollider.radius * 0.1f)) dist = (groundHit.distance - capsuleCollider.radius * 0.1f);
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.1f, out groundHit, groundLayer);
                }
            }

            groundDistance = (float)System.Math.Round(dist, 2);
        }
    }

    public virtual float GroundAngle(){
        var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
        return groundAngle;
    }
    public virtual bool jumpFwdCondition
    {
        get
        {
            Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * capsuleCollider.height * 0.5F;
            Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;
            return Physics.CapsuleCastAll(p1, p2, capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
        }
    }

    public void SubscribeBehaviour(GenericBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }
    public void RegisterDefaultBehaviour (int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    public void RegisterBehaviour(int behaviourCode){
        if(currentBehaviour == defaultBehaviour)
        {
            currentBehaviour = behaviourCode;
        }
    }
    public void UnregisterBehaviour(int behaviourCode){
        if(currentBehaviour == behaviourCode)
        {
            currentBehaviour = defaultBehaviour;
        }
    }

    public bool OverrideWithBehaviour(GenericBehaviour behaviour)
    {
        if (!overridingbehaviours.Contains(behaviour))
        {
            if(overridingbehaviours.Count == 0)
            {
                foreach(GenericBehaviour overriddenBehaviour in behaviours)
                {
                    overriddenBehaviour.OnOverridde();
                    break;
                }
            }
            overridingbehaviours.Add(behaviour);
            return false;
        }
        return false;
    }
    public bool RevokeOverridingBehaviour(GenericBehaviour behaviour){

        if(overridingbehaviours.Contains(behaviour)){
            overridingbehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    // Check if any or a specific behaviour is currently overriding the active one.
    public bool IsOverriding(GenericBehaviour behaviour = null){

        if (behaviour == null)
            return overridingbehaviours.Count > 0;
        return overridingbehaviours.Contains(behaviour);
    }

    // Check if the active behaviour is the passed one.
    public bool IsCurrentBehaviour(int behaviourCode){

        return this.currentBehaviour == behaviourCode;
    }

    // Check if any other behaviour is temporary locked.
    public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0){

        return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
    }

    // Atempt to lock on a specific behaviour.
    //  No other behaviour can overrhide during the temporary lock.
    // Use for temporary transitions like jumping, entering/exiting aiming mode, etc.
    public void LockTempBehaviour(int behaviourCode){

        if(behaviourLocked == 0){
            behaviourLocked = behaviourCode;
        }
    }

    // Attempt to unlock the current locked behaviour.
    // Use after a temporary transition ends.
    public void UnlockTempBehaviour(int behaviourCode){

        if(behaviourLocked == behaviourCode){

            behaviourLocked = 0;
        }
    }

    public bool IsMoving(){

        return (h != 0) || (v != 0);
    }

    public bool NotMoving(){
        return (h == 0) || (v == 0);
    }

    // Get the last player direction of facing.
    public Vector3 GetLastDirection(){

        return lastDirection;
    }

    // Set the last player direction of facing.
    public void SetLastDirection(Vector3 direction){

        lastDirection = direction;
    }

    // Put the player on a standing up position based on last direction faced.
    public void Repositioning(){
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(mybody.rotation, targetRotation, turningSmoothing);
            mybody.MoveRotation(newRotation);
        }
    }

    public virtual void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
    {
        if (animator.isMatchingTarget || animator.IsInTransition(0))
            return;

        float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

        if (normalizeTime > normalisedEndTime)
            return;

        animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
    }

    public void DisableGravityAndCollision()
    {
        animator.SetFloat("HorizontalInput", 0f);
        animator.SetFloat("VerticalInput", 0f);
        mybody.useGravity = false;
        capsuleCollider.isTrigger = true;
    }

    /// <summary>
    /// Turn rigidbody gravity on the uncheck the capsulle collider as Trigger when the animation has finish playing
    /// </summary>
    /// <param name="normalizedTime">Check the value of your animation Exit Time and insert here</param>
    public void EnableGravityAndCollision(float normalizedTime)
    {
        // enable collider and gravity at the end of the animation
        if (baseLayerInfo.normalizedTime >= normalizedTime)
        {
            capsuleCollider.isTrigger = false;
            mybody.useGravity = true;
        }
    }


    public bool IsAnimatorTag(string tag){

        if (animator == null) return false;
        if (baseLayerInfo.IsTag(tag)) return true;
        if (underBodyInfo.IsTag(tag)) return true;
        if (upperBodyInfo.IsTag(tag)) return true;

        return false;
    }
}

public abstract class GenericBehaviour : MonoBehaviour
{
    protected int SpeedFloat;
    protected int CrouchFloat;
    protected BehaviourController behaviourController;
    public int behaviourCode;
    
    void Awake(){
        behaviourController = GetComponent<BehaviourController>();
        SpeedFloat = Animator.StringToHash("Speed");
        CrouchFloat = Animator.StringToHash("CrouchSpeed");
    }

    public virtual void LocalFixedUpdate(){}

    public virtual void LocalLateUpdate(){}

    public virtual void  OnOverridde(){}

    public int GetBehaviourCode() {
        return behaviourCode;
    }
}
